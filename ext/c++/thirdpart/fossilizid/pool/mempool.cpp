/*
 * mempool.h
 *
 *  Created on: 2014-8-18
 *      Author: qianqians
 */
#include "mempool.h"

#ifdef _WINDOWS
#include <Windows.h>
#endif

namespace Fossilizid{
namespace pool {

std::vector<block * > mempool::vpool;
int mempool::corenum;

mempool __mempool__;

static int chunck_normal_len = 1048576;

static bool bInit = false;
static boost::mutex mu;

static int _1K = 32768;
static int _2K = 65536;
static int _4K = 4096;
static int _8K = 8192;
static int _16K = 16384;
static int _32K = 32768;
static int _64K = 65536;

inline int standardization_lenght(int & len){
	if (len <= 8){
		len = 8;
		return 0;
	}else if (len <= 32){
		len = 32;
		return 1;
	}else if (len <= 128){
		len = 128;
		return 2;
	}else if (len <= 512){
		len = 512;
		return 3;
	}else if (len <= _1K){
		len = _1K;
		return 4;
	}else if (len <= _2K){
		len = _2K;
		return 5;
	}else if (len <= _4K){
		len = _4K;
		return 6;
	} else if (len <= _8K){
		len = _8K;
		return 7;
	} else if (len <= _16K){
		len = _16K;
		return 8;
	} else if (len <= _32K){
		len = _32K;
		return 9;
	} else if (len <= _64K){
		len = _64K;
		return 10;
	} else{
		len = (len + _4K - 1) / _4K * _4K;
	}

	return -1;
}

mempool::mempool(){
#ifdef ___mempool___
	if (bInit == false){
		boost::mutex::scoped_lock lock(mu);
		if (bInit == true){
			return;
		}

		mempool::corenum = 8;
#ifdef _WINDOWS
		SYSTEM_INFO info;
		GetSystemInfo(&info);
		mempool::corenum = info.dwNumberOfProcessors;
#endif
		mempool::vpool.resize(mempool::corenum);
		for (int i = 0; i < mempool::corenum; i++){
			mempool::vpool[i] = new block;
			mempool::vpool[i]->recvlist[0] = 0;
			mempool::vpool[i]->recvlist[1] = 0;
			mempool::vpool[i]->recvlist[2] = 0;
			mempool::vpool[i]->recvlist[3] = 0;
			mempool::vpool[i]->recvlist[4] = 0;
			mempool::vpool[i]->recvlist[5] = 0;
			mempool::vpool[i]->recvlist[6] = 0;
			mempool::vpool[i]->recvlist[7] = 0;
			mempool::vpool[i]->recvlist[8] = 0;
			mempool::vpool[i]->recvlist[9] = 0;
			mempool::vpool[i]->recvlist[10] = 0;
		}
	}
#endif
}

mempool::~mempool(){
#ifdef ___mempool___
	if (bInit == true){
		boost::mutex::scoped_lock lock(mu);

		for (int i = 0; i < mempool::corenum; i++){
			delete mempool::vpool[i];
		}
		mempool::vpool.clear();
	}
#endif
}

void * mempool::allocator(int len){
#ifdef ___mempool___
	int recvlistindex = standardization_lenght(len);
	void * mem = 0;
	for (int index = 0; index < mempool::corenum; index++){
		boost::mutex::scoped_try_lock lock(mempool::vpool[index]->mu);
		if (lock.owns_lock()){
			if (recvlistindex >= 0){
				if (len <= _4K && mempool::vpool[index]->recvlist[recvlistindex] != 0 && mem == 0){
					mem = mempool::vpool[index]->recvlist[recvlistindex];
					mempool::vpool[index]->recvlist[recvlistindex] = *((char**)mem);
				}
			}else{
				if (mem == 0){
					std::map<int, char * >::iterator find = mempool::vpool[index]->recvlistchunk.lower_bound(len);
					if (find != mempool::vpool[index]->recvlistchunk.end()){
						mem = find->second;
					}
				}
			}

			if (mem != 0){
				break;
			}
		}
	}

	if (mem == 0){
		mem = malloc(len);
	}

	memset(mem, 0, len);

	return mem;
#else

	return malloc(len);

#endif //___mempool___
}

void mempool::deallocator(void * buff, int len){
#ifdef ___mempool___
	int recvlistindex = standardization_lenght(len);
	int index = 0;
	while (1){
		boost::mutex::scoped_try_lock lock(mempool::vpool[index]->mu);
		if (lock.owns_lock()){
			if (recvlistindex >= 0){
				*((char**)buff) = mempool::vpool[index]->recvlist[recvlistindex];
				mempool::vpool[index]->recvlist[recvlistindex] = (char*)buff;
			} else {
				mempool::vpool[index]->recvlistchunk.insert(std::make_pair(len, (char*)(buff)));
			}
			break;
		}

		if (++index == mempool::corenum){
			index = 0;
		}
	}
#else

	free(buff);

#endif //___mempool___
}	

} /* namespace pool */
} /* namespace Fossilizid */