/*
 * qianqians
 * 2016/7/4
 * Ichannel.h
 */
#ifndef _Ichannel_h
#define _Ichannel_h

#include <vector>

#include <boost/any.hpp>
#include <boost/shared_ptr.hpp>

namespace juggle {

class Ichannel {
public:
	virtual bool pop(boost::shared_ptr<std::vector<boost::any> >  &) = 0;
	virtual void push(boost::shared_ptr<std::vector<boost::any> >) = 0;
	
};

}

#endif // !_Ichannel_h