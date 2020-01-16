/*this module file is codegen by juggle for c++*/
#ifndef _dbproxy_call_hub_module_h
#define _dbproxy_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class dbproxy_call_hub : public juggle::Imodule {
public:
    dbproxy_call_hub(){
        module_name = "dbproxy_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", std::bind(&dbproxy_call_hub::reg_hub_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_create_persisted_object", std::bind(&dbproxy_call_hub::ack_create_persisted_object, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_updata_persisted_object", std::bind(&dbproxy_call_hub::ack_updata_persisted_object, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_count", std::bind(&dbproxy_call_hub::ack_get_object_count, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_info", std::bind(&dbproxy_call_hub::ack_get_object_info, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_info_end", std::bind(&dbproxy_call_hub::ack_get_object_info_end, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_remove_object", std::bind(&dbproxy_call_hub::ack_remove_object, this, std::placeholders::_1)));
    }

    ~dbproxy_call_hub(){
    }

    boost::signals2::signal<void() > sig_reg_hub_sucess;
    void reg_hub_sucess(Fossilizid::JsonParse::JsonArray _event){
        sig_reg_hub_sucess();
    }

    boost::signals2::signal<void(std::string, bool) > sig_ack_create_persisted_object;
    void ack_create_persisted_object(Fossilizid::JsonParse::JsonArray _event){
        sig_ack_create_persisted_object(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<bool>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string) > sig_ack_updata_persisted_object;
    void ack_updata_persisted_object(Fossilizid::JsonParse::JsonArray _event){
        sig_ack_updata_persisted_object(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, int64_t) > sig_ack_get_object_count;
    void ack_get_object_count(Fossilizid::JsonParse::JsonArray _event){
        sig_ack_get_object_count(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<int64_t>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string, Fossilizid::JsonParse::JsonArray) > sig_ack_get_object_info;
    void ack_get_object_info(Fossilizid::JsonParse::JsonArray _event){
        sig_ack_get_object_info(
            std::any_cast<std::string>((*_event)[0]), 
            std::any_cast<Fossilizid::JsonParse::JsonArray>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string) > sig_ack_get_object_info_end;
    void ack_get_object_info_end(Fossilizid::JsonParse::JsonArray _event){
        sig_ack_get_object_info_end(
            std::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_ack_remove_object;
    void ack_remove_object(Fossilizid::JsonParse::JsonArray _event){
        sig_ack_remove_object(
            std::any_cast<std::string>((*_event)[0]));
    }

};

}

#endif
