/*
 * ringque.h
 *  Created on: 2012-1-13
 *      Author: qianqians
 * ringque
 */
#ifndef _RINGQUE_H
#define _RINGQUE_H

#include <atomic>
#include <shared_mutex>

#include "../pool/objpool.h"

namespace Fossilizid{
namespace container{

template<typename T, typename _Allocator = std::allocator<T>, unsigned detailsize = 1024 >
class ringque{ 
private:
	typedef typename _Allocator::template rebind<std::atomic<typename T *> >::other _Allque;
	typedef std::atomic<typename T *> * que;

public:
	ringque(){
		_que = get_que(detailsize);
		
		_push_slide.store(0);
		_que_max = detailsize;
		_pop_slide.store(_que_max);
		_size.store(0);
	}

	~ringque(){
		put_que(_que, _que_max);
	}
		
	/*
	 * que is empty
	 */
	bool empty(){
		return (size() == 0);
	}
		
	/*
	 * get que size
	 */
	std::size_t size(){
		return _size.load();
	}

	/*
	 * clear this que
	 */
	void clear(){
		std::unique_lock<std::shared_mutex> lock(_mu);

		boost::atomic<typename T *> * _tmp = _que;
		boost::uint32_t _tmp_push_slide = _push_slide.load();
		boost::uint32_t _tmp_pop_slide = _pop_slide.load();

		_que = get_que(_que_max);
		_push_slide.store(0);
		_pop_slide.store(_que_max);
		_size.store(0);

		lock.unlock();

		while(1){
			if (_tmp_pop_slide == _que_max){
				_tmp_pop_slide = 0;
			}

			if (_tmp_pop_slide != _tmp_push_slide){
				break;
			}

			_tmp[_tmp_pop_slide]->~T();
			pool::objpool<T>::deallocator(_tmp[_tmp_pop_slide], 1);
		}
		put_que(_tmp, _que_max);
	}
		
	/*
	 * push a element to que
	 */
	void push(T data){
		std::shared_lock<std::shared_mutex> lock(_mu);

		unsigned int slide = _push_slide.load();
		unsigned int newslide = 0;
		while(1){
			newslide = slide+1; 
			while (newslide == _pop_slide.load()){
				lock.unlock();

				{
					std::unique_lock<std::shared_mutex> uniquelock(_mu, std::try_to_lock);
					if (uniquelock.owns_lock()){
						if (newslide == _pop_slide.load()){
							resize();
						}
					}
				}

				lock.lock();
			}

			if (newslide == _que_max){
				newslide = 0;
			}

			if (_push_slide.compare_exchange_strong(slide, newslide)){	
				T * _null = 0, *_node = pool::objpool<T>::allocator(1);
				new (_node)T(data);
				while(!_que[slide].compare_exchange_weak(_null, _node)){
					_null = 0;
				}
				_size++;
				break;
			}
		}
	}

	/*
	 * pop a element form que if empty return false 
	 */
	bool pop(T & data){
		std::shared_lock<std::shared_mutex> lock(_mu);

		T * _tmp = 0;
		unsigned int slide = _pop_slide.load(), slidetail = ((slide == _que_max) ? 0 : slide);
		unsigned int newslide = 0;
		while(1){
			newslide = slidetail + 1;
			
			if (slidetail == _push_slide.load()){
				break;
			}

			if (_pop_slide.compare_exchange_strong(slide, newslide)){
				while((_tmp = _que[slidetail].exchange(0)) == 0);
				data = *_tmp;
				pool::objpool<T>::deallocator(_tmp, 1);
				_size--;
				return true;
			}
		}

		return false;
	}

private:
	void resize(){
		size_t size = 0;
		unsigned int pushslide = _push_slide.load();
		unsigned int popslide = _pop_slide.load();
		unsigned int slidetail = ((popslide == _que_max) ? 0 : popslide);
		std::atomic<typename T *> * _tmp = 0;

		size = _que_max*2;
		_tmp = get_que(size);
		if (popslide >= pushslide){
			for(size_t i = 0; i < pushslide; i++){
				_tmp[i].store(_que[i].load(std::memory_order_relaxed), std::memory_order_relaxed);
			}
			size_t size1 = _que_max - popslide;
			for(size_t i = 0; i < size1; i++){
				_tmp[i+size-size1].store(_que[popslide+i].load(std::memory_order_relaxed), std::memory_order_relaxed);
			}
			_pop_slide += _que_max;
		}else{
			for(size_t i = popslide; i < pushslide; i++){
				_tmp[i].store(_que[i].load(std::memory_order_relaxed), std::memory_order_relaxed);
			}
		}
		put_que(_que, _que_max);
		_que_max *= 2;
		_que = _tmp;
	}

private:
	que get_que(uint32_t size){
		que _que = _que_alloc.allocate(size);
		while(_que == 0){
			_que = _que_alloc.allocate(size);
		}
		for(uint32_t i = 0; i < size; i++){
			_que[i].store(0, std::memory_order_relaxed);		
		}

		return _que;
	}

	void put_que(que _que, uint32_t size){
		_que_alloc.deallocate(_que, size);
	}

private:
	std::shared_mutex _mu;
	que _que;
	std::atomic_uint _push_slide, _pop_slide, _size;
	unsigned int _que_max;
	
	_Allque _que_alloc;

};

}//container
}//Fossilizid

#endif //_RINGQUE_H