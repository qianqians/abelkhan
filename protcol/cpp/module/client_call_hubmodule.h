/*this module file is codegen by juggle for c++*/
#ifndef _client_call_hub_module_h
#define _client_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class client_call_hub : public juggle::Imodule {
public:
    client_call_hub(){
        module_name = "client_call_hub";
        protcolcall_set.insert(std::make_pair("client_connect", std::bind(&client_call_hub::client_connect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("call_hub", std::bind(&client_call_hub::call_hub, this, std::placeholders::_1)));
    }

    ~client_call_hub(){
    }

    boost::signals2::signal<void(std::string) > sig_client_connect;
    void client_connect(Fossilizid::JsonParse::JsonArray _event){
        sig_client_connect(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, Fossilizid::JsonParse::JsonArray) > sig_call_hub;
    void call_hub(Fossilizid::JsonParse::JsonArray _event){
        sig_call_hub(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<std::string>((*_event)[1]), 
            std::any_cast<std::string>((*_event)[2]), 
            std::any_cast<Fossilizid::JsonParse::JsonArray>((*_event)[3]));
    }

};

}

#endif
