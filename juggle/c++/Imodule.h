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

#include <msgpack.hpp>

#include "Ichannel.h"

namespace juggle {

class Imodule {
public:
	void process_event(boost::shared_ptr<Ichannel> _ch, msgpack::object _event) {
		current_ch = _ch;

		auto func_name = std::get<1>(_event.as<std::tuple<std::string, std::string, msgpack::object>>());
		auto func = protcolcall_set.find(func_name);
		if (func != protcolcall_set.end()) {
			func->second(std::get<2>(_event.as<std::tuple<std::string, std::string, msgpack::object>>()));
		}
		else {
			std::cout << "can not find function named:" << func_name << std::endl;
		}

		current_ch = nullptr;
	}

public:
	std::string module_name;

private:
	boost::shared_ptr<Ichannel> current_ch;
	std::unordered_map<std::string, std::function<void(msgpack::object)> > protcolcall_set;

};

}

#endif // !_Imodule_h
