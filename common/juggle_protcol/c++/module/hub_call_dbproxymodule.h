/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_dbproxy_module_h
#define _hub_call_dbproxy_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class hub_call_dbproxy : public juggle::Imodule {
public:
    hub_call_dbproxy(){
        module_name = "hub_call_dbproxy";
        protcolcall_set.insert(std::make_pair("reg_hub", boost::bind(&hub_call_dbproxy::reg_hub, this, _1)));
        protcolcall_set.insert(std::make_pair("create_persisted_object", boost::bind(&hub_call_dbproxy::create_persisted_object, this, _1)));
        protcolcall_set.insert(std::make_pair("updata_persisted_object", boost::bind(&hub_call_dbproxy::updata_persisted_object, this, _1)));
        protcolcall_set.insert(std::make_pair("get_object_info", boost::bind(&hub_call_dbproxy::get_object_info, this, _1)));
    }

    ~hub_call_dbproxy(){
    }

    boost::signals2::signal<void(std::string) > sigreg_hubhandle;
    void reg_hub(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_hubhandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, int64_t) > sigcreate_persisted_objecthandle;
    void create_persisted_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigcreate_persisted_objecthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string, std::string, int64_t) > sigupdata_persisted_objecthandle;
    void updata_persisted_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigupdata_persisted_objecthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]));
    }

    boost::signals2::signal<void(std::string, int64_t) > sigget_object_infohandle;
    void get_object_info(boost::shared_ptr<std::vector<boost::any> > _event){
        sigget_object_infohandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

};

}

#endif
