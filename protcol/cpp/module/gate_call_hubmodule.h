/*this module file is codegen by juggle for c++*/
#ifndef _gate_call_hub_module_h
#define _gate_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class gate_call_hub : public juggle::Imodule {
public:
    gate_call_hub(){
        module_name = "gate_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", std::bind(&gate_call_hub::reg_hub_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_connect", std::bind(&gate_call_hub::client_connect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_disconnect", std::bind(&gate_call_hub::client_disconnect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_exception", std::bind(&gate_call_hub::client_exception, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_call_hub", std::bind(&gate_call_hub::client_call_hub, this, std::placeholders::_1)));
    }

    ~gate_call_hub(){
    }

    boost::signals2::signal<void() > sig_reg_hub_sucess;
    void reg_hub_sucess(Fossilizid::JsonParse::JsonArray _event){
        sig_reg_hub_sucess();
    }

    boost::signals2::signal<void(std::string) > sig_client_connect;
    void client_connect(Fossilizid::JsonParse::JsonArray _event){
        sig_client_connect(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_client_disconnect;
    void client_disconnect(Fossilizid::JsonParse::JsonArray _event){
        sig_client_disconnect(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_client_exception;
    void client_exception(Fossilizid::JsonParse::JsonArray _event){
        sig_client_exception(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, Fossilizid::JsonParse::JsonArray) > sig_client_call_hub;
    void client_call_hub(Fossilizid::JsonParse::JsonArray _event){
        sig_client_call_hub(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<std::string>((*_event)[1]), 
            std::any_cast<std::string>((*_event)[2]), 
            std::any_cast<Fossilizid::JsonParse::JsonArray>((*_event)[3]));
    }

};

}

#endif
