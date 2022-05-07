
#ifndef _modulemng_h
#define _modulemng_h

#include <exception>

#include "module.h"

namespace common
{

class modulemanager {
public:
	void add_module(std::string module_name, std::shared_ptr<imodule> module)
	{
		modules.insert(std::make_pair(module_name, module));
	}

	void process_module_mothed(std::string& module_name, std::string& cb_name, msgpack11::MsgPack::array& InArray)
	{
		auto module = modules.find(module_name);
		if (module != modules.end())
		{
			(module->second)->invoke(cb_name, InArray);
		}
		else {
			spdlog::error("modulemanager.process_module_mothed unreg module name:{0}!", module_name);
			throw moduleException(std::format("modulemanager.process_module_mothed unreg module name:{0}!", module_name));
		}
	}

private:
	std::map<std::string, std::shared_ptr<imodule> > modules;

};

}

#endif
