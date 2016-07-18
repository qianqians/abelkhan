/*this module file is codegen by juggle for c++*/
#ifndef _logic_call_dbproxy_module_h
#define _logic_call_dbproxy_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class logic_call_dbproxy : public juggle::Imodule {
public:
    logic_call_dbproxy(){
        module_name = "logic_call_dbproxy";
        protcolcall_set.insert(std::make_pair("reg_logic", boost::bind(&logic_call_dbproxy::reg_logic, this, _1)));
        protcolcall_set.insert(std::make_pair("create_persisted_object", boost::bind(&logic_call_dbproxy::create_persisted_object, this, _1)));
        protcolcall_set.insert(std::make_pair("updata_persisted_object", boost::bind(&logic_call_dbproxy::updata_persisted_object, this, _1)));
        protcolcall_set.insert(std::make_pair("get_object_info", boost::bind(&logic_call_dbproxy::get_object_info, this, _1)));
        protcolcall_set.insert(std::make_pair("logic_closed", boost::bind(&logic_call_dbproxy::logic_closed, this, _1)));
    }

    ~logic_call_dbproxy(){
    }

    boost::signals2::signal<void(std::string) > sigreg_logichandle;
    void reg_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logichandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string) > sigcreate_persisted_objecthandle;
    void create_persisted_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigcreate_persisted_objecthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > sigupdata_persisted_objecthandle;
    void updata_persisted_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigupdata_persisted_objecthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

    boost::signals2::signal<void(std::string, std::string) > sigget_object_infohandle;
    void get_object_info(boost::shared_ptr<std::vector<boost::any> > _event){
        sigget_object_infohandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

    boost::signals2::signal<void() > siglogic_closedhandle;
    void logic_closed(boost::shared_ptr<std::vector<boost::any> > _event){
        siglogic_closedhandle(
);
    }

};

}

#endif
