
#ifndef _module_h
#define _module_h

#include <string>
#include <memory>
#include <vector>
#include <map>
#include <functional>
#include <any>

#include <JsonParse.h>

namespace common
{

class imodule {
protected:
	std::map<std::string, std::function< void( Fossilizid::JsonParse::JsonArray ) > > cbs;

	void reg_cb(std::string cb_name, std::function< void(Fossilizid::JsonParse::JsonArray) > cb)
	{
		cbs.insert(std::make_pair(cb_name, cb));
	}

public:
	void invoke(std::string cb_name, Fossilizid::JsonParse::JsonArray InArray)
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
