
#ifndef _module_h
#define _module_h

#include <string>
#include <memory>
#include <vector>
#include <map>
#include <functional>

#include <spdlog/spdlog.h>
#include <msgpack11.hpp>

namespace common
{

class imodule {
protected:
	std::map<std::string, std::function< void(msgpack11::MsgPack::array) > > cbs;

	void reg_cb(std::string cb_name, std::function< void(msgpack11::MsgPack::array) > cb)
	{
		cbs.insert(std::make_pair(cb_name, cb));
	}

public:
	void invoke(std::string cliient_cuuid, std::string cb_name, msgpack11::MsgPack::array InArray)
	{
		auto cb = cbs.find(cb_name);
		if (cb != cbs.end())
		{
			current_client_uuid = cliient_cuuid;
			(cb->second)(InArray);
			current_client_uuid = "";
		}
		else {
			spdlog::error("imodule.invoke unreg func name:{0}", cb_name);
		}
	}

public:
	std::string current_client_uuid;

};

}

#endif
