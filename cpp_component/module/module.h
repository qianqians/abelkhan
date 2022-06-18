
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

class Response
{
};

class imodule {
public:
    thread_local static std::shared_ptr<Response> rsp;
};

}

#endif
