/*
 * qianqians
 * 2016/7/4
 * Icaller.h
 */
#ifndef _Icaller_h
#define _Icaller_h

#include <boost/shared_ptr.hpp>

#include "Ichannel.h"

namespace juggle {
	
class Icaller {
public:
	Icaller(boost::shared_ptr<Ichannel> _ch) {
		ch = _ch;
	}

protected:
	boost::shared_ptr<Ichannel> ch;
	std::string module_name;

};

}

#endif // !_Icaller_h