/*this module file is codegen by juggle for c++*/
#ifndef _dbproxy_call_logic_module_h
#define _dbproxy_call_logic_module_h
#include <Imodule.h>
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class dbproxy_call_logic : public juggle::Imodule {
public:
    dbproxy_call_logic(){
        module_name = "dbproxy_call_logic";
        protcolcall_set.insert(std::make_pair("reg_logic_sucess", std::bind(&dbproxy_call_logic::reg_logic_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_create_persisted_object", std::bind(&dbproxy_call_logic::ack_create_persisted_object, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_updata_persisted_object", std::bind(&dbproxy_call_logic::ack_updata_persisted_object, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_info", std::bind(&dbproxy_call_logic::ack_get_object_info, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_info_end", std::bind(&dbproxy_call_logic::ack_get_object_info_end, this, std::placeholders::_1)));
    }

    ~dbproxy_call_logic(){
    }

    boost::signals2::signal<void() > sigreg_logic_sucesshandle;
    void reg_logic_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logic_sucesshandle(
);
    }

    boost::signals2::signal<void(std::string) > sigack_create_persisted_objecthandle;
    void ack_create_persisted_object(std::shared_ptr<std::vector<boost::any> > _event){
        sigack_create_persisted_objecthandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sigack_updata_persisted_objecthandle;
    void ack_updata_persisted_object(std::shared_ptr<std::vector<boost::any> > _event){
        sigack_updata_persisted_objecthandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string) > sigack_get_object_infohandle;
    void ack_get_object_info(std::shared_ptr<std::vector<boost::any> > _event){
        sigack_get_object_infohandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string) > sigack_get_object_info_endhandle;
    void ack_get_object_info_end(std::shared_ptr<std::vector<boost::any> > _event){
        sigack_get_object_info_endhandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

};

}

#endif
