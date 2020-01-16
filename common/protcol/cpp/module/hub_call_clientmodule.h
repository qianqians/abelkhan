/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_client_module_h
#define _hub_call_client_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class hub_call_client : public juggle::Imodule {
public:
    hub_call_client(){
        module_name = "hub_call_client";
        protcolcall_set.insert(std::make_pair("call_client", std::bind(&hub_call_client::call_client, this, std::placeholders::_1)));
    }

    ~hub_call_client(){
    }

    boost::signals2::signal<void(std::string, std::string, Fossilizid::JsonParse::JsonArray) > sig_call_client;
    void call_client(Fossilizid::JsonParse::JsonArray _event){
        sig_call_client(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<std::string>((*_event)[1]), 
            std::any_cast<Fossilizid::JsonParse::JsonArray>((*_event)[2]));
    }

};

}

#endif
