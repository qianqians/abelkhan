/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_hub_module_h
#define _hub_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class hub_call_hub : public juggle::Imodule {
public:
    hub_call_hub(){
        module_name = "hub_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub", std::bind(&hub_call_hub::reg_hub, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", std::bind(&hub_call_hub::reg_hub_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("hub_call_hub_mothed", std::bind(&hub_call_hub::hub_call_hub_mothed, this, std::placeholders::_1)));
    }

    ~hub_call_hub(){
    }

    boost::signals2::signal<void(std::string) > sig_reg_hub;
    void reg_hub(Fossilizid::JsonParse::JsonArray _event){
        sig_reg_hub(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sig_reg_hub_sucess;
    void reg_hub_sucess(Fossilizid::JsonParse::JsonArray _event){
        sig_reg_hub_sucess();
    }

    boost::signals2::signal<void(std::string, std::string, Fossilizid::JsonParse::JsonArray) > sig_hub_call_hub_mothed;
    void hub_call_hub_mothed(Fossilizid::JsonParse::JsonArray _event){
        sig_hub_call_hub_mothed(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<std::string>((*_event)[1]), 
            std::any_cast<Fossilizid::JsonParse::JsonArray>((*_event)[2]));
    }

};

}

#endif
