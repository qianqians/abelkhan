/*this module file is codegen by juggle for c++*/
#ifndef _client_call_gate_module_h
#define _client_call_gate_module_h
#include <Imodule.h>
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
        protcolcall_set.insert(std::make_pair("forward_client_call_logic", std::bind(&client_call_gate::forward_client_call_logic, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("heartbeats", std::bind(&client_call_gate::heartbeats, this, std::placeholders::_1)));
    }

    ~client_call_gate(){
    }

    boost::signals2::signal<void(std::string) > sigconnect_serverhandle;
    void connect_server(std::shared_ptr<std::vector<boost::any> > _event){
        sigconnect_serverhandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sigcancle_serverhandle;
    void cancle_server(std::shared_ptr<std::vector<boost::any> > _event){
        sigcancle_serverhandle(
);
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sigforward_client_call_logichandle;
    void forward_client_call_logic(std::shared_ptr<std::vector<boost::any> > _event){
        sigforward_client_call_logichandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[2]));
    }

    boost::signals2::signal<void() > sigheartbeatshandle;
    void heartbeats(std::shared_ptr<std::vector<boost::any> > _event){
        sigheartbeatshandle(
);
    }

};

}

#endif
