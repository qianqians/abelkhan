/*
 * qianqians
 * 2016/7/4
 * Ichannel.h
 */
#ifndef _Ichannel_h
#define _Ichannel_h

#include <msgpack.hpp>

namespace juggle {

class Ichannel {
public:
	virtual msgpack::object pop() = 0;
	virtual void senddata(char * data, int datasize) = 0;
	
};

}

#endif // !_Ichannel_h