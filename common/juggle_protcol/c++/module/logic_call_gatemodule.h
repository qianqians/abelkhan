/*this module file is codegen by juggle for c++*/
#ifndef _logic_call_gate_module_h
#define _logic_call_gate_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class logic_call_gate : public juggle::Imodule {
public:
    logic_call_gate(){
        module_name = "logic_call_gate";
        protcolcall_set.insert(std::make_pair("reg_logic", boost::bind(&logic_call_gate::reg_logic, this, _1)));
        protcolcall_set.insert(std::make_pair("ack_client_connect_server", boost::bind(&logic_call_gate::ack_client_connect_server, this, _1)));
        protcolcall_set.insert(std::make_pair("forward_logic_call_client", boost::bind(&logic_call_gate::forward_logic_call_client, this, _1)));
        protcolcall_set.insert(std::make_pair("forward_logic_call_group_client", boost::bind(&logic_call_gate::forward_logic_call_group_client, this, _1)));
        protcolcall_set.insert(std::make_pair("forward_logic_call_global_client", boost::bind(&logic_call_gate::forward_logic_call_global_client, this, _1)));
    }

    ~logic_call_gate(){
    }

    boost::signals2::signal<void(std::string) > sigreg_logichandle;
    void reg_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logichandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string) > sigack_client_connect_serverhandle;
    void ack_client_connect_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_client_connect_serverhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, boost::shared_ptr<std::vector<boost::any> >) > sigforward_logic_call_clienthandle;
    void forward_logic_call_client(boost::shared_ptr<std::vector<boost::any> > _event){
        sigforward_logic_call_clienthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<boost::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

    boost::signals2::signal<void(boost::shared_ptr<std::vector<boost::any> >, std::string, std::string, boost::shared_ptr<std::vector<boost::any> >) > sigforward_logic_call_group_clienthandle;
    void forward_logic_call_group_client(boost::shared_ptr<std::vector<boost::any> > _event){
        sigforward_logic_call_group_clienthandle(
            boost::any_cast<boost::shared_ptr<std::vector<boost::any> >>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<boost::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

    boost::signals2::signal<void(std::string, std::string, boost::shared_ptr<std::vector<boost::any> >) > sigforward_logic_call_global_clienthandle;
    void forward_logic_call_global_client(boost::shared_ptr<std::vector<boost::any> > _event){
        sigforward_logic_call_global_clienthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<boost::shared_ptr<std::vector<boost::any> >>((*_event)[2]));
    }

};

}

#endif
