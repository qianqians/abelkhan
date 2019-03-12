/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_gate_module_h
#define _hub_call_gate_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class hub_call_gate : public juggle::Imodule {
public:
    hub_call_gate(){
        module_name = "hub_call_gate";
        protcolcall_set.insert(std::make_pair("reg_hub", std::bind(&hub_call_gate::reg_hub, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("connect_sucess", std::bind(&hub_call_gate::connect_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("disconnect_client", std::bind(&hub_call_gate::disconnect_client, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("forward_hub_call_client", std::bind(&hub_call_gate::forward_hub_call_client, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("forward_hub_call_group_client", std::bind(&hub_call_gate::forward_hub_call_group_client, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("forward_hub_call_global_client", std::bind(&hub_call_gate::forward_hub_call_global_client, this, std::placeholders::_1)));
    }

    ~hub_call_gate(){
    }

    boost::signals2::signal<void(std::string, std::string) > sig_reg_hub;
    void reg_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_hub(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string) > sig_connect_sucess;
    void connect_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sig_connect_sucess(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_disconnect_client;
    void disconnect_client(std::shared_ptr<std::vector<boost::any> > _event){
        sig_disconnect_client(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_forward_hub_call_client;
    void forward_hub_call_client(std::shared_ptr<std::vector<boost::any> > _event){
        sig_forward_hub_call_client(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

    boost::signals2::signal<void(std::shared_ptr<std::vector<boost::any> >, std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_forward_hub_call_group_client;
    void forward_hub_call_group_client(std::shared_ptr<std::vector<boost::any> > _event){
        sig_forward_hub_call_group_client(
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_forward_hub_call_global_client;
    void forward_hub_call_global_client(std::shared_ptr<std::vector<boost::any> > _event){
        sig_forward_hub_call_global_client(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[2]));
    }

};

}

#endif
