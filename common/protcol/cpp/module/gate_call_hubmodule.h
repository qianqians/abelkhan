/*this module file is codegen by juggle for c++*/
#ifndef _gate_call_hub_module_h
#define _gate_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class gate_call_hub : public juggle::Imodule {
public:
    gate_call_hub(){
        module_name = "gate_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", std::bind(&gate_call_hub::reg_hub_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_connect", std::bind(&gate_call_hub::client_connect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_disconnect", std::bind(&gate_call_hub::client_disconnect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_exception", std::bind(&gate_call_hub::client_exception, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_call_hub", std::bind(&gate_call_hub::client_call_hub, this, std::placeholders::_1)));
    }

    ~gate_call_hub(){
    }

    boost::signals2::signal<void() > sig_reg_hub_sucess;
    void reg_hub_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_hub_sucess();
    }

    boost::signals2::signal<void(std::string) > sig_client_connect;
    void client_connect(std::shared_ptr<std::vector<boost::any> > _event){
        sig_client_connect(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_client_disconnect;
    void client_disconnect(std::shared_ptr<std::vector<boost::any> > _event){
        sig_client_disconnect(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_client_exception;
    void client_exception(std::shared_ptr<std::vector<boost::any> > _event){
        sig_client_exception(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_client_call_hub;
    void client_call_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_client_call_hub(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

};

}

#endif
