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

#include <abelkhan.h>

#include "hub.h"

namespace hub {

class hub_service;
class hubproxy {
public:
	hubproxy(std::shared_ptr<hub_service> _hub_, std::string hub_name, std::string hub_type, std::shared_ptr<abelkhan::Ichannel> hub_ch);

	void call_hub(const std::string& func_name, const msgpack11::MsgPack::array& argvs);

	void client_seep(std::string client_uuid);

public:
	std::string _hub_name;
	std::string _hub_type;

	std::shared_ptr<abelkhan::Ichannel> _hub_ch;

private:
	std::shared_ptr<hub_service> _hub;
	std::shared_ptr<abelkhan::hub_call_hub_caller> _hub_call_hub_caller;

};

class hubsvrmanager {
public:
	hubsvrmanager(std::shared_ptr<hub_service> _hub_);

	void reg_hub(std::string hub_name, std::string hub_type, std::shared_ptr<abelkhan::Ichannel> ch);

	void call_hub(const std::string& hub_name, const std::string& func_name, const msgpack11::MsgPack::array& argvs);

	std::shared_ptr<hubproxy> get_hub(std::shared_ptr<abelkhan::Ichannel> ch);

public:
	std::shared_ptr<hubproxy> current_hubproxy = nullptr;

private:
	std::unordered_map<std::string, std::shared_ptr<hubproxy> > hubproxys;
	std::unordered_map<std::shared_ptr<abelkhan::Ichannel>, std::shared_ptr<hubproxy> > ch_hubproxys;

	std::shared_ptr<hub_service> _hub;

};

}

#endif //_hubsvrmanager_h
