/*this module file is codegen by juggle for c++*/
#ifndef _client_call_gate_module_h
#define _client_call_gate_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class client_call_gate : public juggle::Imodule {
public:
    client_call_gate(){
        module_name = "client_call_gate";
        protcolcall_set.insert(std::make_pair("connect_server", std::bind(&client_call_gate::connect_server, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("cancle_server", std::bind(&client_call_gate::cancle_server, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("connect_hub", std::bind(&client_call_gate::connect_hub, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("enable_heartbeats", std::bind(&client_call_gate::enable_heartbeats, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("disable_heartbeats", std::bind(&client_call_gate::disable_heartbeats, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("forward_client_call_hub", std::bind(&client_call_gate::forward_client_call_hub, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("heartbeats", std::bind(&client_call_gate::heartbeats, this, std::placeholders::_1)));
    }

    ~client_call_gate(){
    }

    boost::signals2::signal<void(std::string, int64_t) > sig_connect_server;
    void connect_server(std::shared_ptr<std::vector<boost::any> > _event){
        sig_connect_server(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

    boost::signals2::signal<void() > sig_cancle_server;
    void cancle_server(std::shared_ptr<std::vector<boost::any> > _event){
        sig_cancle_server();
    }

    boost::signals2::signal<void(std::string) > sig_connect_hub;
    void connect_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_connect_hub(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sig_enable_heartbeats;
    void enable_heartbeats(std::shared_ptr<std::vector<boost::any> > _event){
        sig_enable_heartbeats();
    }

    boost::signals2::signal<void() > sig_disable_heartbeats;
    void disable_heartbeats(std::shared_ptr<std::vector<boost::any> > _event){
        sig_disable_heartbeats();
    }

    boost::signals2::signal<void(std::string, std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_forward_client_call_hub;
    void forward_client_call_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_forward_client_call_hub(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

    boost::signals2::signal<void(int64_t) > sig_heartbeats;
    void heartbeats(std::shared_ptr<std::vector<boost::any> > _event){
        sig_heartbeats(
            boost::any_cast<int64_t>((*_event)[0]));
    }

};

}

#endif
