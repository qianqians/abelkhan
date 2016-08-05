/*
 * qianqians
 * 2016/7/4
 * Ichannel.h
 */
#ifndef _Ichannel_h
#define _Ichannel_h

#include <vector>
#include <memory>

#include <boost/any.hpp>

namespace juggle {

class Ichannel {
public:
	virtual bool pop(std::shared_ptr<std::vector<boost::any> >  &) = 0;
	virtual void push(std::shared_ptr<std::vector<boost::any> >) = 0;
	
};

}

#endif // !_Ichannel_h