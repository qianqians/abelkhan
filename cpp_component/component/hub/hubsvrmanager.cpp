/*
 * qianqians
 * 2016-7-12
 * hubsvrmanager.cpp
 */
#include <factory.h>

#include "hubsvrmanager.h"
#include "hub.h"

namespace hub {
	
hubproxy::hubproxy(std::string hub_name, std::shared_ptr<juggle::Ichannel> hub_ch) {
	name = hub_name;
	caller = Fossilizid::pool::factory::create<caller::hub_call_hub>(hub_ch);
}

void hubproxy::reg_hub_sucess() {
	caller->reg_hub_sucess();
}

void hubproxy::call_hub(std::string module_name, std::string func_name, Fossilizid::JsonParse::JsonArray argvs) {
	caller->hub_call_hub_mothed(module_name, func_name, argvs);
}

hubsvrmanager::hubsvrmanager(std::shared_ptr<hub_service> _hub_) {
	_hub = _hub_;
}

std::shared_ptr<hubproxy> hubsvrmanager::reg_hub(std::string hub_name, std::shared_ptr<juggle::Ichannel> ch) {
	auto _proxy = Fossilizid::pool::factory::create<hubproxy>(hub_name, ch);
	hubproxys[hub_name] = _proxy;

	_hub->sig_hub_connect(hub_name);

	return _proxy;
}

void hubsvrmanager::call_hub(std::string hub_name, std::string module_name, std::string func_name, Fossilizid::JsonParse::JsonArray argvs) {
	if (hubproxys.find(hub_name) == hubproxys.end()) {
		return;
	}

	hubproxys[hub_name]->call_hub(module_name, func_name, argvs);
}

}