/*this module file is codegen by juggle for c++*/
#ifndef _gate_call_logic_module_h
#define _gate_call_logic_module_h
#include <Imodule.h>
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class gate_call_logic : public juggle::Imodule {
public:
    gate_call_logic(){
        module_name = "gate_call_logic";
        protcolcall_set.insert(std::make_pair("reg_logic_sucess", std::bind(&gate_call_logic::reg_logic_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_connect", std::bind(&gate_call_logic::client_connect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_disconnect", std::bind(&gate_call_logic::client_disconnect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("client_call_logic", std::bind(&gate_call_logic::client_call_logic, this, std::placeholders::_1)));
    }

    ~gate_call_logic(){
    }

    boost::signals2::signal<void() > sigreg_logic_sucesshandle;
    void reg_logic_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logic_sucesshandle(
);
    }

    boost::signals2::signal<void(std::string) > sigclient_connecthandle;
    void client_connect(std::shared_ptr<std::vector<boost::any> > _event){
        sigclient_connecthandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sigclient_disconnecthandle;
    void client_disconnect(std::shared_ptr<std::vector<boost::any> > _event){
        sigclient_disconnecthandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sigclient_call_logichandle;
    void client_call_logic(std::shared_ptr<std::vector<boost::any> > _event){
        sigclient_call_logichandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

};

}

#endif
