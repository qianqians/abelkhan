
#ifndef _modulemng_h
#define _modulemng_h

#include <exception>

#include <string_tools.h>

#include "module.h"

namespace common
{

class moduleException : public std::exception{
public:
	moduleException(std::string err_) : std::exception(err_.c_str()){
		_err = err_;
	}

public:
	std::string _err;

};

class modulemanager {
public:
	void add_module(std::string module_name, std::shared_ptr<imodule> module)
	{
		modules.insert(std::make_pair(module_name, module));
	}

	void process_module_mothed(std::string& module_name, std::string& cb_name, std::vector<uint8_t>& InArray)
	{
		auto module = modules.find(module_name);
		if (module != modules.end())
		{
			(module->second)->invoke(cb_name, InArray);
		}
		else {
			spdlog::error("modulemanager.process_module_mothed unreg module name:{0}!", module_name);
			throw moduleException(concurrent::format("modulemanager.process_module_mothed unreg module name:%s!", module_name));
		}
	}

private:
	std::map<std::string, std::shared_ptr<imodule> > modules;

};

}

#endif
