/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_dbproxy_module_h
#define _hub_call_dbproxy_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class hub_call_dbproxy : public juggle::Imodule {
public:
    hub_call_dbproxy(){
        module_name = "hub_call_dbproxy";
        protcolcall_set.insert(std::make_pair("reg_hub", std::bind(&hub_call_dbproxy::reg_hub, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("create_persisted_object", std::bind(&hub_call_dbproxy::create_persisted_object, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("updata_persisted_object", std::bind(&hub_call_dbproxy::updata_persisted_object, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("get_object_count", std::bind(&hub_call_dbproxy::get_object_count, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("get_object_info", std::bind(&hub_call_dbproxy::get_object_info, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("remove_object", std::bind(&hub_call_dbproxy::remove_object, this, std::placeholders::_1)));
    }

    ~hub_call_dbproxy(){
    }

    boost::signals2::signal<void(std::string) > sig_reg_hub;
    void reg_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_hub(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::unordered_map<std::string, boost::any> >, std::string) > sig_create_persisted_object;
    void create_persisted_object(std::shared_ptr<std::vector<boost::any> > _event){
        sig_create_persisted_object(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::unordered_map<std::string, boost::any> >, std::shared_ptr<std::unordered_map<std::string, boost::any> >, std::string) > sig_updata_persisted_object;
    void updata_persisted_object(std::shared_ptr<std::vector<boost::any> > _event){
        sig_updata_persisted_object(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[3]), 
            boost::any_cast<std::string>((*_event)[4]));
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::unordered_map<std::string, boost::any> >, std::string) > sig_get_object_count;
    void get_object_count(std::shared_ptr<std::vector<boost::any> > _event){
        sig_get_object_count(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::unordered_map<std::string, boost::any> >, std::string) > sig_get_object_info;
    void get_object_info(std::shared_ptr<std::vector<boost::any> > _event){
        sig_get_object_info(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::unordered_map<std::string, boost::any> >, std::string) > sig_remove_object;
    void remove_object(std::shared_ptr<std::vector<boost::any> > _event){
        sig_remove_object(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

};

}

#endif
