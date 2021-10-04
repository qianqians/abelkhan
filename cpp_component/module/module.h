
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
	void invoke(std::string& cb_name, std::vector<uint8_t>& data)
	{
		auto cb = cbs.find(cb_name);
		if (cb != cbs.end())
		{
			std::string err;
			auto InArray = msgpack11::MsgPack::parse((char*)data.data(), data.size(), err);
			if (!InArray.is_array()) {
				spdlog::trace("call_hub argv is not match!!");
				throw moduleException(concurrent::format("call_hub argv is not match:%s!", cb_name));
			}

			(cb->second)(InArray.array_items());
		}
		else {
			spdlog::error("imodule.invoke unreg func name:{0}", cb_name);
			throw moduleException(concurrent::format("imodule.invoke unreg func name:%s!", cb_name));
		}
	}

};

}

#endif
