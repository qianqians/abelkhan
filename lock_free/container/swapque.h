/*
 * swapque.h
 *  Created on: 2013-1-16
 *	    Author: qianqians
 * swapque:a fifo que
 */
#ifndef _SWAPQUE_H
#define _SWAPQUE_H

#include <boost/thread/shared_mutex.hpp>
#include <boost/atomic.hpp>

#include "../container/detail/_hazard_ptr.h"
#include "../pool/objpool.h"

namespace Fossilizid{
namespace container{

template <typename T, typename _Allocator = std::allocator<T> >
class swapque{
private:
	struct _que_node{
		_que_node () : _next(0) {}
		_que_node (const T & val) : data(val), _next(0) {}
		~_que_node () {}

		T data;
		boost::atomic<_que_node *> _next;
	};

	struct _mirco_que{
		boost::atomic<_que_node *> _begin;
		boost::atomic<_que_node *> _end;
	};

	struct _que{
		_mirco_que * _frond, * _back;
		boost::atomic_uint32_t _size;
		boost::shared_mutex _mu;
	};

	typedef Fossilizid::container::detail::_hazard_ptr<_que_node> _hazard_ptr;
	typedef Fossilizid::container::detail::_hazard_system<_que_node> _hazard_system;
	typedef Fossilizid::container::detail::_hazard_ptr<_que> _hazard_que_ptr;
	typedef Fossilizid::container::detail::_hazard_system<_que> _hazard_que_system;
	typedef typename _Allocator::template rebind<_que_node>::other _node_alloc;
	typedef typename _Allocator::template rebind<_mirco_que>::other _mirco_que_alloc;
	typedef typename _Allocator::template rebind<_que>::other _que_alloc;

public:
	swapque() : _hazard_sys(boost::bind(&swapque::put_node, this, _1)), _hazard_que_sys(boost::bind(&swapque::put_que, this, _1)){
		__que.store(get_que());
	}

	~swapque(){
		put_que(__que.load());
	}
		
	/*
	 * que is empty
	 */
	bool empty(){
		return (__que.load()->_size.load() == 0);
	}
		
	/*
	 * get que size
	 */
	std::size_t size(){
		return __que.load()->_size.load();
	}
		
	/*
	 * clear this que
	 */
	void clear(){
		_que * _new_que = get_que();
		_que * _old_que = __que.exchange(_new_que);
		put_que(_old_que);
	}
		
	/*
	 * push a element to que
	 */
	void push(const T & data){
		_que_node * _node = get_node(data);

		_hazard_que_ptr * _hazard_que;
		_hazard_que_sys.acquire(&_hazard_que, 1);	
		while(1){
			_hazard_que->_hazard = __que.load();
			boost::shared_lock<boost::shared_mutex> lock(_hazard_que->_hazard->_mu, boost::try_to_lock);

			if (lock.owns_lock()){
				_que_node * _old_end = _hazard_que->_hazard->_back->_end.exchange(_node);
				_old_end->_next = _node;
				_hazard_que->_hazard->_size++;

				break;
			}
		}
		_hazard_que_sys.release(_hazard_que);
	}
		
	/*
	 * pop a element form que if empty return false 
	 */
	bool pop(T & data){
		if (__que.load()->_size.load() == 0){
			return false;
		}

		bool bRet = false;

		_hazard_que_ptr * _hazard_que;
		_hazard_que_sys.acquire(&_hazard_que, 1);	
		_hazard_ptr * _hp_node[2];
		_hazard_sys.acquire(_hp_node, 2);
		while(1){
			_hazard_que->_hazard = __que.load();
			boost::shared_lock<boost::shared_mutex> lock(_hazard_que->_hazard->_mu, boost::try_to_lock);
			
			if (lock.owns_lock()){
				_hp_node[0]->_hazard = _hazard_que->_hazard->_frond->_begin.load();
				while(1){
					while (_hp_node[0]->_hazard == _hazard_que->_hazard->_frond->_end.load()){
						if (_hazard_que->_hazard->_size.load() == 0){
							goto end;
						}

						lock.unlock();

						{
							boost::unique_lock<boost::shared_mutex> uniquelock(_hazard_que->_hazard->_mu, boost::try_to_lock);
							if (uniquelock.owns_lock()){
								std::swap(_hazard_que->_hazard->_frond, _hazard_que->_hazard->_back);
							}
						}

						lock.lock();
						_hp_node[0]->_hazard = _hazard_que->_hazard->_frond->_begin.load();
					}

					_hp_node[1]->_hazard = _hp_node[0]->_hazard->_next;
					if(_hazard_que->_hazard->_frond->_begin.compare_exchange_strong(_hp_node[0]->_hazard, _hp_node[1]->_hazard)){
						data = _hp_node[1]->_hazard->data;
						_hazard_sys.retire(_hp_node[0]->_hazard);
						_hazard_que->_hazard->_size--;
						bRet = true;
						goto end;
					}
				}
			}
		}

	end:
		_hazard_sys.release(_hp_node[0]);
		_hazard_sys.release(_hp_node[1]);

		_hazard_que_sys.release(_hazard_que);

		return bRet;
	}

private:
	_que * get_que(){
		_que * __que = __que_alloc.allocate(1);
		while(__que == 0){__que = __que_alloc.allocate(1);};
		::new(__que) _que();

		__que->_size = 0;

		__que->_frond = __mirco_que_alloc.allocate(1);
		_que_node * _node = pool::objpool<_que_node>::allocator(1);
		new (_node) _que_node();
		__que->_frond->_begin.store(_node);
		__que->_frond->_end.store(_node);

		__que->_back = __mirco_que_alloc.allocate(1);
		_node = pool::objpool<_que_node>::allocator(1);
		new (_node)_que_node();
		__que->_back->_begin.store(_node);
		__que->_back->_end.store(_node);

		return __que;
	}

	void put_que(_que * _p){
		boost::unique_lock<boost::shared_mutex> lock(_p->_mu);

		_que_node * _node = _p->_frond->_begin;
		do{
			_que_node * _tmp = _node;
			_node = _node->_next;

			_hazard_sys.retire(_tmp);
		}while(_node != 0);
		__mirco_que_alloc.deallocate(_p->_frond, 1);

		_node = _p->_back->_begin;
		do{
			_que_node * _tmp = _node;
			_node = _node->_next;

			_hazard_sys.retire(_tmp);
		}while(_node != 0);
		__mirco_que_alloc.deallocate(_p->_back, 1);

		lock.unlock();
		_p->~_que();
		__que_alloc.deallocate(_p, 1);
	}

	_que_node * get_node(const T & data){
		_que_node * _node = pool::objpool<_que_node>::allocator(1);
		new (_node)_que_node(data);
		return _node;
	}

	void put_node(_que_node * _p){
		pool::objpool<_que_node>::deallocator(_p, 1);
	}

private:
	boost::atomic<_que *> __que;
	_que_alloc __que_alloc;
	_mirco_que_alloc __mirco_que_alloc;

	_hazard_system _hazard_sys;
	_hazard_que_system _hazard_que_sys;

};	
	
} //container
} //Fossilizid

#endif //_SWAPQUE_H