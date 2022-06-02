
#ifndef _modulemng_h
#define _modulemng_h

#include <exception>

#include "module.h"

namespace common
{

class modulemanager {
protected:
	std::unordered_map<std::string, std::function< void(msgpack11::MsgPack::array) > > motheds;

public:
	void add_mothed(std::string mothed_name, std::function< void(msgpack11::MsgPack::array) > mothed)
	{
		motheds.insert(std::make_pair(mothed_name, mothed));
	}

	void process_module_mothed(const std::string& mothed_name, msgpack11::MsgPack::array& InArray)
	{
		auto mothed = motheds.find(mothed_name);
		if (mothed != motheds.end())
		{
			(mothed->second)(InArray);
		}
		else {
			spdlog::error("modulemanager.process_module_mothed unreg mothed name:{0}!", mothed_name);
			throw moduleException(std::format("modulemanager.process_module_mothed unreg mothed name:{0}!", mothed_name));
		}
	}


};

}

#endif
