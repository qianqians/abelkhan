/*
 * objpool.h
 *
 *  Created on: 2014-8-18
 *      Author: qianqians
 */
#ifndef _objpool_h
#define _objpool_h

#include "mempool.h"

namespace Fossilizid{
namespace pool{

template<typename T>
class objpool{
public:
	objpool(){
	}

	~objpool(){
	}

public:

	static T * allocator(int len){
		return (T*)mempool::allocator(sizeof(T)*len);
	}

	static void deallocator(T * buff, int count){
		for (int i = 0; i < count; i++){
			buff[i].~T();
		}

		mempool::deallocator(buff, sizeof(T)*count);
	}

private:

};

} /* namespace pool */
} /* namespace Fossilizid */

#endif //_objpool_h