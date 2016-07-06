/*
 * qianqians
 * 2016/7/4
 * Ichannel.h
 */
#ifndef _Ichannel_h
#define _Ichannel_h

#include <msgpack.hpp>

#include <boost/shared_ptr.hpp>

namespace juggle {

class Ichannel {
public:
	virtual bool pop(std::string &) = 0;
	virtual void senddata(char * data, int datasize) = 0;
	
};

}

#endif // !_Ichannel_h