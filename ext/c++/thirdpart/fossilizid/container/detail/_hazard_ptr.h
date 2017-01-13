/*
 * _hazard_ptr.hpp
 *  Created on: 2012-8-26
 *	    Author: qianqians
 * _hazard_ptr: Used to solve the ABA problem
 */

#ifndef _HAZARD_PTR_H
#define _HAZARD_PTR_H

#include <array>
#include <thread>
#include <functional>
#include <atomic>
#include <vector>

namespace Fossilizid{
namespace container{
namespace detail{
	
// _hazard_ptr
template <typename X>
struct _hazard_ptr{
	X * _hazard; //
	std::atomic_int32_t _active; // 0 使用中/1 未使用
};

// hazard system 
template <typename T, typename _Allocator = std::allocator<T> >
class _hazard_system{
private:
	//Recover flag
	std::atomic_flag recoverflag;

	// deallocate function
	typedef std::function<void(typename T * )> fn_dealloc;
	// deallocate struct data
	typedef typename T * _deallocate_data;
	typedef typename _Allocator::template rebind<_deallocate_data>::other _Allocator_deallocate_data;
	// recover list
	struct recover_list {
		recover_list() : active(1) {re_vector.reserve(32);}
		~recover_list(){}

		std::vector<_deallocate_data, _Allocator_deallocate_data> re_vector;
		std::atomic_int32_t active; // 0 使用中 / 1 未使用
	};

	// allocator 
	typedef typename _Allocator::template rebind<recover_list>::other __alloc_recover_list;

	__alloc_recover_list _alloc_recover_list;

	// 回收队列集合
	std::array<recover_list * , 8> re_list_set;

	fn_dealloc _fn_dealloc;

	// lock-free list(push only) storage _hazard_ptr
	typedef _hazard_ptr<typename T> _hazard_ptr_;
	typedef struct _list_node{
		_hazard_ptr_ _hazard;
		std::atomic<_list_node *> next;
	} _list_head;

	// allocator 
	typedef typename _Allocator::template rebind<_list_node>::other __alloc_list_node;

	__alloc_list_node _alloc_list_node;

	// hazard ptr list
	std::atomic<_list_head *> _head;
	// list lenght
	std::atomic_uint32_t llen;

public:
	_hazard_system(fn_dealloc _D) : _fn_dealloc(_D){
		for(int i = 0; i < 8; i++){
			re_list_set[i] = _alloc_recover_list.allocate(1);
			new(re_list_set[i]) recover_list();
		}

		llen.store(1);
		_head = _get_node();
	}

	~_hazard_system(){
		for(recover_list * _re_list : re_list_set){
			if(!_re_list->re_vector.empty()){
				for(_deallocate_data var : _re_list->re_vector){
					_fn_dealloc(var);
				}
				_re_list->re_vector.clear();

				_re_list->~recover_list();
				_alloc_recover_list.deallocate(_re_list, 1);
			}
		}
	}

private:
	_list_node * _get_node(){
		_list_node * _node = _alloc_list_node.allocate(1);
		_node->_hazard._hazard = 0;
		_node->_hazard._active = 1;
		_node->next = 0;

		return _node;
	}

	void _put_node(_list_node * _node){
		_alloc_list_node.deallocate(_node, 1);
	}
	
public:
	void acquire(_hazard_ptr_ ** ptr, uint32_t size ){
		_list_node * _node = _head; 
		while(size > 0){
			// try to reuse a retired hazard ptr
			for( ; _node; _node = _node->next){
				if (_node->_hazard._active.exchange(0) == 1){
					break;
				}
			}

			if (!_node){
				// alloc a new node 
				_node = _get_node();
				_node->_hazard._active.store(0);
				// increment the list length
				llen++;
				// push into list
				_node->next = _head.exchange(_node);
			}

			ptr[--size] = &(_node->_hazard);
		}
	}

	void release(_hazard_ptr_ * ptr){
		ptr->_hazard = 0;
		ptr->_active.store(1);
	}

public:
	void retire(T * p){
		// get tss rvector
		recover_list * _rvector_ptr = 0;
		while(1){
			for(int i = 0; i < 8; i++) {
				if (re_list_set[i]->active.exchange(0) == 0){
					continue;
				}else{
					_rvector_ptr = re_list_set[i];
					break;
				}
			}

			if (_rvector_ptr != 0){
				break;
			}
		}

		// scan
		if(_rvector_ptr->re_vector.size() >= 32 && _rvector_ptr->re_vector.size() > llen.load()){
			// scan hazard pointers list collecting all non-null ptrs
			std::vector<void *> hvector;
			for(_list_node * _node = _head; _node; _node = _node->next){
				void * p = _node->_hazard._hazard;
				if (p != 0)
					hvector.push_back(p);
			}

			// sort hazard list
			std::sort(hvector.begin(), hvector.end(), std::less<void*>());
		
			// deallocator
			std::vector<_deallocate_data>::iterator iter = _rvector_ptr->re_vector.begin();
			while(iter != _rvector_ptr->re_vector.end()){
				if(!std::binary_search(hvector.begin(), hvector.end(), *iter)){
					_fn_dealloc(*iter);

					if(*iter != _rvector_ptr->re_vector.back()){
						*iter = _rvector_ptr->re_vector.back();
						_rvector_ptr->re_vector.pop_back();
					}
					else{
						iter = _rvector_ptr->re_vector.erase(iter);
					}
				}
				else{
					++iter;
				}
			}
		}

		// push into rvector
		_rvector_ptr->re_vector.push_back(p);
		
		_rvector_ptr->active.store(1);
	}

};

} /* detail */
} /* container */
} /* Fossilizid */

#endif // _HAZARD_PTR_H

