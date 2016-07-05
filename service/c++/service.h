/*
 * qianqians
 * 2016/7/4
 * service.h
 */
#ifndef _service_h
#define _service_h

#include <cstdint>

namespace service {

class service{
public:
	service(){
	}

	~service(){
	}

	virtual void poll(int64_t tick) = 0;

};

}

#endif // !_service_h
