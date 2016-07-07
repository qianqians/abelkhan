/*
 * qianqians
 * 2016/7/4
 * Imodule.h
 */
#ifndef _Imodule_h
#define _Imodule_h

#include <boost/shared_ptr.hpp>

#include <functional>
#include <unordered_map>
#include <iostream>

#include "Ichannel.h"

namespace juggle {

class Imodule {
public:
	void process_event(boost::shared_ptr<Ichannel> _ch, boost::shared_ptr<std::vector<boost::any> > _event) {
		current_ch = _ch;

		auto func_name = boost::any_cast<std::string>((*_event)[1]);
		auto func = protcolcall_set.find(func_name);
		if (func != protcolcall_set.end()) {
			func->second(boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*_event)[2]));
		}
		else {
			std::cout << "can not find function named:" << func_name << std::endl;
		}

		current_ch = nullptr;
	}

public:
	std::string module_name;

protected:
	std::unordered_map<std::string, std::function<void(boost::shared_ptr<std::vector<boost::any> > )> > protcolcall_set;

private:
	boost::shared_ptr<Ichannel> current_ch;

};

}

#endif // !_Imodule_h
