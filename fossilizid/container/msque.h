/*
 * msque.hpp
 *		 Created on: 2012-8-30
 *			 Author: qianqians
 * msque:
 */

#ifndef _MSQUE_H
#define _MSQUE_H

#include <functional>
#include <atomic>

#include "../container/detail/_hazard_ptr.h"
#include "../pool/objpool.h"

namespace Fossilizid {
namespace container{

template <typename T, typename _Allocator = std::allocator<T> >
class msque{
private:
	struct _list_node{
		_list_node () {_next = 0;}
		_list_node (const T & val) : data(val) {_next = 0;}
		~_list_node () {}

		T data;
		std::atomic<_list_node *> _next;
	};

	struct _list{
		std::atomic<_list_node *> _begin;
		std::atomic<_list_node *> _end;
		std::atomic_uint32_t _size;
	};
	
	typedef Fossilizid::container::detail::_hazard_ptr<_list_node> _hazard_ptr;
	typedef Fossilizid::container::detail::_hazard_system<_list_node> _hazard_system;
	typedef Fossilizid::container::detail::_hazard_ptr<_list> _hazard_list_ptr;
	typedef Fossilizid::container::detail::_hazard_system<_list> _hazard_list_;
	typedef typename _Allocator::template rebind<_list_node>::other _node_alloc;
	typedef typename _Allocator::template rebind<_list>::other _list_alloc;
		
public:
	msque(void) : _hazard_sys(std::bind(&msque::put_node, this, std::placeholders::_1)), _hazard_list(std::bind(&msque::put_list, this, std::placeholders::_1)){
		__list.store(get_list());
	}

	~msque(void){
		put_list(__list.load());
	}

	/*
	 * que is empty
	 */
	bool empty(){
		return (__list.load()->_size.load() == 0);
	}

	/*
	 * get que size
	 */
	std::size_t size(){
		return __list.load()->_size.load();
	}

	/*
	 * clear this que
	 */
	void clear(){
		_list * _new_list = get_list();
		_list * _old_list = __list.exchange(_new_list);
		put_list(_old_list);
	}

	/*
	 * push a element to que
	 */
	void push(const T & data){
		_list_node * _node = get_node(data);

		_hazard_list_ptr * _hp_list;
		_hazard_list.acquire(&_hp_list, 1);
		_hazard_ptr * _hp;
		_hazard_sys.acquire(&_hp, 1);
		while(1){
			_hp_list->_hazard = __list.load();
			_hp->_hazard = _hp_list->_hazard->_end.load();

			_list_node * next = _hp->_hazard->_next.load();			 
			if(next != 0){
				_hp_list->_hazard->_end.compare_exchange_weak(_hp->_hazard, next);
				continue;
			}

			if(_hp->_hazard != _hp_list->_hazard->_end.load()){
				continue;
			}

			if (_hp->_hazard->_next.compare_exchange_weak(next, _node)){
				_hp_list->_hazard->_end.compare_exchange_weak(_hp->_hazard, _node); 
				_hp_list->_hazard->_size++;
				break;
			}
		}

		_hazard_sys.release(_hp);
		_hazard_list.release(_hp_list);
	}

	/*
	 * pop a element form que if empty return false 
	 */
	bool pop(T & data){
		bool ret = true;
		
		_hazard_list_ptr * _hp_list;
		_hazard_list.acquire(&_hp_list, 1);
		_hazard_ptr * _hp_node[2];
		_hazard_sys.acquire(_hp_node, 2);
		while(1){	
			_hp_list->_hazard = __list.load();

			_hp_node[0]->_hazard = _hp_list->_hazard->_begin.load();
			_hp_node[1]->_hazard = _hp_node[0]->_hazard->_next.load();
			
			if(_hp_node[1]->_hazard == 0){
				ret = false;
				break;
			}

			if(_hp_node[0]->_hazard != _hp_list->_hazard->_begin.load()){
				_hp_node[0]->_hazard = _hp_list->_hazard->_begin.load();
				continue;
			}

			if(_hp_list->_hazard->_begin.compare_exchange_strong(_hp_node[0]->_hazard, _hp_node[1]->_hazard)){
				data = _hp_node[1]->_hazard->data;
				_hazard_sys.retire(_hp_node[0]->_hazard);
				_hp_list->_hazard->_size--;
				break;
			}
		}

		_hazard_list.release(_hp_list);
		_hazard_sys.release(_hp_node[0]);
		_hazard_sys.release(_hp_node[1]);

		return ret;
	}

private:
	_list * get_list(){
		_list * __list = __list_alloc.allocate(1);
		while(__list == 0){__list = __list_alloc.allocate(1);}
		new (__list) _list();

		__list->_size = 0;

		_list_node * _node = get_node();
		_node->_next.store(0);
		__list->_begin.store(_node);
		__list->_end.store(_node);

		return __list;
	}

	void put_list(_list * _p){
		_list_node * _node = _p->_begin;
		do{
			_list_node * _tmp = _node;
			_node = _node->_next;

			_hazard_sys.retire(_tmp);
		}while(_node != 0);
		__list_alloc.deallocate(_p, 1);
	}

	_list_node * get_node(){
		_list_node * _node = pool::objpool<_list_node>::allocator(1);
		new (_node) _list_node();

		return _node;
	}

	_list_node * get_node(const T & data){
		_list_node * _node = pool::objpool<_list_node>::allocator(1);
		new (_node) _list_node(data);

		return _node;
	}

	void put_node(_list_node * _p){
		pool::objpool<_list_node>::deallocator(_p, 1);
	}

private:
	std::atomic<_list *> __list;
	_list_alloc __list_alloc;
	_node_alloc __node_alloc;

	_hazard_system _hazard_sys;
	_hazard_list_ _hazard_list;

};

} /* Hemsleya */
} /* container */
#endif //_MSQUE_H