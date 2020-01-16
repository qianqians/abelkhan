/*this module file is codegen by juggle for c++*/
#ifndef _center_call_server_module_h
#define _center_call_server_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class center_call_server : public juggle::Imodule {
public:
    center_call_server(){
        module_name = "center_call_server";
        protcolcall_set.insert(std::make_pair("reg_server_sucess", std::bind(&center_call_server::reg_server_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("close_server", std::bind(&center_call_server::close_server, this, std::placeholders::_1)));
    }

    ~center_call_server(){
    }

    boost::signals2::signal<void() > sig_reg_server_sucess;
    void reg_server_sucess(Fossilizid::JsonParse::JsonArray _event){
        sig_reg_server_sucess();
    }

    boost::signals2::signal<void() > sig_close_server;
    void close_server(Fossilizid::JsonParse::JsonArray _event){
        sig_close_server();
    }

};

}

#endif
