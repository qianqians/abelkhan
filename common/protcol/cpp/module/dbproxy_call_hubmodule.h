/*this module file is codegen by juggle for c++*/
#ifndef _dbproxy_call_hub_module_h
#define _dbproxy_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
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
    void reg_hub_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_hub_sucess();
    }

    boost::signals2::signal<void(std::string, bool) > sig_ack_create_persisted_object;
    void ack_create_persisted_object(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_create_persisted_object(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<bool>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string) > sig_ack_updata_persisted_object;
    void ack_updata_persisted_object(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_updata_persisted_object(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, int64_t) > sig_ack_get_object_count;
    void ack_get_object_count(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_get_object_count(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string, std::shared_ptr<std::vector<boost::any> >) > sig_ack_get_object_info;
    void ack_get_object_info(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_get_object_info(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string) > sig_ack_get_object_info_end;
    void ack_get_object_info_end(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_get_object_info_end(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_ack_remove_object;
    void ack_remove_object(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_remove_object(
            boost::any_cast<std::string>((*_event)[0]));
    }

};

}

#endif
