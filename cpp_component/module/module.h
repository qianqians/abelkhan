
#ifndef _module_h
#define _module_h

#include <string>
#include <memory>
#include <vector>
#include <map>
#include <functional>

#include <spdlog/spdlog.h>
#include <msgpack11.hpp>

#include <string_tools.h>

namespace common
{

class moduleException : public std::exception {
public:
	moduleException(std::string err_) : std::exception(err_.c_str()) {
		_err = err_;
	}

public:
	std::string _err;

};

class imodule {
protected:
	std::map<std::string, std::function< void(msgpack11::MsgPack::array) > > cbs;

	void reg_cb(std::string cb_name, std::function< void(msgpack11::MsgPack::array) > cb)
	{
		cbs.insert(std::make_pair(cb_name, cb));
	}

public:
	void invoke(std::string& cb_name, msgpack11::MsgPack::array& InArray)
	{
		auto cb = cbs.find(cb_name);
		if (cb != cbs.end())
		{
			(cb->second)(InArray);
		}
		else {
			spdlog::error("imodule.invoke unreg func name:{0}", cb_name);
			throw moduleException(concurrent::format("imodule.invoke unreg func name:%s!", cb_name));
		}
	}

};

}

#endif
