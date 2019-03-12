/*this module file is codegen by juggle for c++*/
#ifndef _gate_call_client_module_h
#define _gate_call_client_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class gate_call_client : public juggle::Imodule {
public:
    gate_call_client(){
        module_name = "gate_call_client";
        protcolcall_set.insert(std::make_pair("ntf_uuid", std::bind(&gate_call_client::ntf_uuid, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("connect_gate_sucess", std::bind(&gate_call_client::connect_gate_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("connect_hub_sucess", std::bind(&gate_call_client::connect_hub_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("ack_heartbeats", std::bind(&gate_call_client::ack_heartbeats, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("call_client", std::bind(&gate_call_client::call_client, this, std::placeholders::_1)));
    }

    ~gate_call_client(){
    }

    boost::signals2::signal<void(std::string) > sig_ntf_uuid;
    void ntf_uuid(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ntf_uuid(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sig_connect_gate_sucess;
    void connect_gate_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sig_connect_gate_sucess();
    }

    boost::signals2::signal<void(std::string) > sig_connect_hub_sucess;
    void connect_hub_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sig_connect_hub_sucess(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sig_ack_heartbeats;
    void ack_heartbeats(std::shared_ptr<std::vector<boost::any> > _event){
        sig_ack_heartbeats();
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_call_client;
    void call_client(std::shared_ptr<std::vector<boost::any> > _event){
        sig_call_client(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[2]));
    }

};

}

#endif
