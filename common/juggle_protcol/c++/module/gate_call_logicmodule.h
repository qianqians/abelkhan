/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class gate_call_logic : public juggle::Imodule {
public:
    gate_call_logic(){
        module_name = "gate_call_logic";
        protcolcall_set.insert(std::make_pair("client_connect", boost::bind(&gate_call_logic::client_connect, this, _1)));
        protcolcall_set.insert(std::make_pair("client_disconnect", boost::bind(&gate_call_logic::client_disconnect, this, _1)));
        protcolcall_set.insert(std::make_pair("client_call_logic", boost::bind(&gate_call_logic::client_call_logic, this, _1)));
    }

    ~gate_call_logic(){
    }

    boost::signals2::signal<void(std::string) > sigclient_connecthandle;
    void client_connect(boost::shared_ptr<std::vector<boost::any> > _event){
        sigclient_connecthandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sigclient_disconnecthandle;
    void client_disconnect(boost::shared_ptr<std::vector<boost::any> > _event){
        sigclient_disconnecthandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, std::string) > sigclient_call_logichandle;
    void client_call_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sigclient_call_logichandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

};

}
