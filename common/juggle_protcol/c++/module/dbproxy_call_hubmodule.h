/*this module file is codegen by juggle for c++*/
#ifndef _dbproxy_call_hub_module_h
#define _dbproxy_call_hub_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class dbproxy_call_hub : public juggle::Imodule {
public:
    dbproxy_call_hub(){
        module_name = "dbproxy_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", boost::bind(&dbproxy_call_hub::reg_hub_sucess, this, _1)));
        protcolcall_set.insert(std::make_pair("ack_updata_persisted_object", boost::bind(&dbproxy_call_hub::ack_updata_persisted_object, this, _1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_info", boost::bind(&dbproxy_call_hub::ack_get_object_info, this, _1)));
        protcolcall_set.insert(std::make_pair("ack_get_object_info_end", boost::bind(&dbproxy_call_hub::ack_get_object_info_end, this, _1)));
    }

    ~dbproxy_call_hub(){
    }

    boost::signals2::signal<void() > sigreg_hub_sucesshandle;
    void reg_hub_sucess(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_hub_sucesshandle(
);
    }

    boost::signals2::signal<void(int64_t) > sigack_updata_persisted_objecthandle;
    void ack_updata_persisted_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_updata_persisted_objecthandle(
            boost::any_cast<int64_t>((*_event)[0]));
    }

    boost::signals2::signal<void(int64_t, std::string) > sigack_get_object_infohandle;
    void ack_get_object_info(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_get_object_infohandle(
            boost::any_cast<int64_t>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

    boost::signals2::signal<void(int64_t) > sigack_get_object_info_endhandle;
    void ack_get_object_info_end(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_get_object_info_endhandle(
            boost::any_cast<int64_t>((*_event)[0]));
    }

};

}

#endif
