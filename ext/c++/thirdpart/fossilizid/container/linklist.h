/*
 * linklist
 *  Created on: 2016-11-16
 *	    Author: qianqians
 * linklist:a link list
 */
#ifndef _linklist_H
#define _linklist_H

#include <shared_mutex>
#include <atomic>

#include "../container/detail/_hazard_ptr.h"
#include "../pool/objpool.h"

namespace Fossilizid{
namespace container{

template <typename T, typename _Allocator = std::allocator<T> >
class linklist{
private:
	struct _list_node{
        _list_node () : _next(0) { _flag = 0; }
		_list_node (const T & val) : data(val), _next(0) { _flag = 0; }
		~_list_node () {}

		T data;
        std::atomic_uint32_t _flag;
		std::atomic<_list_node *> _next;
	};

	struct _linklist{
        std::atomic<_list_node *> _detail;
		std::atomic_uint32_t _size;
		std::shared_mutex _mu;
	};

    typedef Fossilizid::container::detail::_hazard_ptr<_linklist> _hazard_list_ptr;
    typedef Fossilizid::container::detail::_hazard_system<_linklist> _hazard_list_system;
    typedef Fossilizid::container::detail::_hazard_ptr<_list_node> _hazard_ptr;
	typedef Fossilizid::container::detail::_hazard_system<_list_node> _hazard_system;
	typedef typename _Allocator::template rebind<_list_node>::other _node_alloc;
	typedef typename _Allocator::template rebind<_list_node>::other _list_alloc;

public:
	linklist() : _hazard_sys(std::bind(&linklist::put_node, this, std::placeholders::_1)), _hazard_list_sys(std::bind(&linklist::put_linklist, this, std::placeholders::_1)){
		__linklist.store(get_linklist());
	}

	~linklist(){
		put_linklist(__linklist.load());
	}
		
	/*
	 * que is empty
	 */
	bool empty(){
		return (__linklist.load()->_size.load() == 0);
	}
		
	/*
	 * get que size
	 */
	std::size_t size(){
		return __linklist.load()->_size.load();
	}
		
	/*
	 * clear this que
	 */
	void clear(){
		_linklist * _new_linklist = get_linklist();
		_linklist * _old_linklist = __linklist.exchange(_new_linklist);
		put_linklist(_old_linklist);
	}
		
	/*
	 * push a element to que
	 */
	void insert(const T & data){
		_list_node * _node = get_node(data);
        
        _hazard_list_ptr * _hazard_list;
        _hazard_list_sys.acquire(&_hazard_list, 1);
        _hazard_ptr * _hp_node[2];
        _hazard_sys.acquire(_hp_node, 2);
        while(1){
			_hazard_list->_hazard = __linklist.load();
			std::shared_lock<std::shared_mutex> lock(_hazard_list->_hazard->_mu, std::try_to_lock);

			if (lock.owns_lock()){
				_hp_node[0]->_hazard = _hazard_list->_hazard->_detail.load();
                while(_hp_node[0]->_hazard->_next != 0){
                    _hp_node[1]->_hazard = _hp_node[0]->_hazard->_next;
                    if (_hp_node[0]->_hazard->data < data && _hp_node[1]->_hazard->data  >= data){
                        break;
                    }
                }
                
                _node->next = _hp_node[1]->_hazard;
                if (_hp_node[0]->_hazard->_next.compare_exchange_strong(_hp_node[1]->_hazard, _node)){
                    _hazard_list->_hazard->_size++;
                    break;
                }
			}
        }
        _hazard_sys.release(_hp_node[0]);
        _hazard_sys.release(_hp_node[1]);
		_hazard_list_sys.release(_hazard_list);
	}
    
    /*
     * find element form list if list not have this data return null
     */
    _hazard_ptr * find(T data){
        if (__linklist.load()->_size.load() == 0){
            return null;
        }
        
        _hazard_list_ptr * _hazard_list;
        _hazard_list_sys.acquire(&_hazard_list, 1);
        _hazard_ptr * _hp_node;
        _hazard_sys.acquire(&_hp_node, 1);
        while(1){
            _hazard_list->_hazard = __linklist.load();
            std::shared_lock<std::shared_mutex> lock(_hazard_list->_hazard->_mu, std::try_to_lock);
            
            if (lock.owns_lock()){
                _hp_node[0]->_hazard = _hazard_list->_hazard->_detail.load();
                while(1){
                    while (_hp_node[0]->_hazard == _hazard_list->_hazard->_detail.load()){
                        if (_hazard_list->_hazard->_size.load() == 0){
                            goto end;
                        }
                    }
                    
                    _hp_node->_hazard = _hazard_list->_hazard->_detail.load();
                    while(_hp_node->_hazard->_next != 0){
                        if (_hp_node->_hazard->data == data){
                            _hazard_list_sys.release(_hazard_que);
                            return _hp_node;
                        }
                    }
                }
            }
        }
        
    end:
        _hazard_sys.release(_hp_node);
        _hazard_list_sys.release(_hazard_que);
        
        return null;
    }
    
    /*
     * release hazard ptr
     */
    void release_hazard_ptr(_hazard_ptr * __hazard_ptr)
    {
        _hazard_sys.release(__hazard_ptr);
    }
		
	/*
	 * reomve a element form list if list not have this data return false
	 */
	bool remove(T data){
		if (__linklist.load()->_size.load() == 0){
			return false;
		}

		bool bRet = false;

        _hazard_list_ptr * _hazard_list;
        _hazard_list_sys.acquire(&_hazard_list, 1);
        _hazard_ptr * _hp_node[2];
        _hazard_sys.acquire(_hp_node, 2);
        while(1){
			_hazard_list->_hazard = __linklist.load();
			std::shared_lock<std::shared_mutex> lock(_hazard_list->_hazard->_mu, std::try_to_lock);
			
			if (lock.owns_lock()){
				_hp_node[0]->_hazard = _hazard_list->_hazard->_detail.load();
				while(1){
					while (_hp_node[0]->_hazard == _hazard_list->_hazard->_detail.load()){
						if (_hazard_list->_hazard->_size.load() == 0){
							goto end;
						}
                    }

                    _hp_node[0]->_hazard = _hazard_list->_hazard->_detail.load();
                    while(_hp_node[0]->_hazard->_next != 0){
                        _hp_node[1]->_hazard = _hp_node[0]->_hazard->_next;
                        if (_hp_node[0]->_hazard->data < data && _hp_node[1]->_hazard->data  >= data){
                            break;
                        }
                    }
                        
                    _node->next = _hp_node[1]->_hazard;
                    if (_hp_node[0]->_hazard->_size.compare_exchange_strong(0, 1)){
                        bRet = true;
                        goto end;
                    }
                }
			}
		}

	end:
		_hazard_sys.release(_hp_node[0]);
		_hazard_sys.release(_hp_node[1]);

		_hazard_list_sys.release(_hazard_que);

		return bRet;
	}

private:
	_linklist * get_linklist(){
		_linklist * __list = __list_alloc.allocate(1);
		while(__list == 0){__list = __list_alloc.allocate(1);};
		::new(__list) _linklist();
        
        _list_node * __node = __node_alloc.allocate(1);
        while(__node == 0){__node = __node_alloc.allocate(1);}
        ::new(__node) _list_node();
        
		__list->_size = 0;

		
		return __list;
	}

	void put_linklist(_linklist * __list){
		std::unique_lock<std::shared_mutex> lock(__list->_mu);

        _list_node * _node = __list->_detail;
        while(_node->_next != 0){
            _list_node * __node = _node;
            _node = __node->next;
            
            put_node(__node);
        }

		lock.unlock();
		__list->~_linklist();
		__list_alloc.deallocate(__list, 1);
	}

	_que_node * get_node(const T & data){
		_list_node * _node = pool::objpool<_que_node>::allocator(1);
		new (_node)_list_node(data);
		return _node;
	}

	void put_node(_list_node * _p){
        _p->~_list_node();
		pool::objpool<_list_node>::deallocator(_p, 1);
	}

private:
	std::atomic<_linklist *> __linklist;
    
	_node_alloc __node_alloc;
	_list_alloc __list_alloc;

	_hazard_system _hazard_sys;
    _hazard_list_system _hazard_list_sys;

};	
	
} //container
} //Fossilizid

#endif //_linklist_H
