/*
 * mempool.h
 *
 *  Created on: 2014-8-18
 *      Author: qianqians
 */
#ifndef _mempool_h
#define _mempool_h

#include <vector>
#include <map>
#include <boost/thread/mutex.hpp>

namespace Fossilizid{
namespace pool{

struct block{
	char * recvlist[11];
	std::map<int, char * > recvlistchunk;
	boost::mutex mu;
};

class mempool{
public:
	mempool();
	~mempool();

public:
	/*
	 * allocator a memory length = len
	 */
	static void * allocator(int len);

	/*
	 * deallocator a memory length = len
	 */
	static void deallocator(void * buff, int len);

private:
	static std::vector<block * > vpool;
	static int corenum;

};

} /* namespace pool */
} /* namespace Fossilizid */

#endif //_mempool_h