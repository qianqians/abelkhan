
#ifndef _module_h
#define _module_h

#include <string>
#include <memory>
#include <vector>
#include <map>
#include <functional>

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
	void invoke(std::string cb_name, msgpack11::MsgPack::array InArray)
	{
		auto cb = cbs.find(cb_name);
		if (cb != cbs.end())
		{
			(cb->second)(InArray);
		}
	}

};

}

#endif
