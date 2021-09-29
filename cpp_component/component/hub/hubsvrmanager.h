/*
 * hubsvrmanager.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hubsvrmanager_h
#define _hubsvrmanager_h

#include <string>
#include <map>
#include <unordered_map>
#include <memory>

#include <absl/container/node_hash_map.h>

#include <Ichannel.h>

#include "hub_call_hubcaller.h"

namespace hub {

class hubproxy {
public:
	hubproxy(std::string hub_name, std::shared_ptr<juggle::Ichannel> hub_ch);

	void reg_hub_sucess();

	void call_hub(std::string module_name, std::string func_name, Fossilizid::JsonParse::JsonArray argvs);

private:
	std::string name;
	std::shared_ptr<caller::hub_call_hub> caller;

};

class hub_service;
class hubsvrmanager {
public:
	hubsvrmanager(std::shared_ptr<hub_service> _hub_);

	std::shared_ptr<hubproxy> reg_hub(std::string hub_name, std::shared_ptr<juggle::Ichannel> ch);

	void call_hub(std::string hub_name, std::string module_name, std::string func_name, Fossilizid::JsonParse::JsonArray argvs);

private:
	absl::node_hash_map<std::string, std::shared_ptr<hubproxy> > hubproxys;
	std::shared_ptr<hub_service> _hub;

};

}

#endif //_hubsvrmanager_h
