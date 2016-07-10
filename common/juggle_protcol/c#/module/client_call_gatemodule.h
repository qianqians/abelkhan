/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class client_call_gate : public juggle::Imodule {
public:
    client_call_gate(){
        module_name = "client_call_gate";
        protcolcall_set.insert(std::make_pair("connect_server", boost::bind(&client_call_gate::connect_server, this, _1)));
        protcolcall_set.insert(std::make_pair("cancle_server", boost::bind(&client_call_gate::cancle_server, this, _1)));
        protcolcall_set.insert(std::make_pair("forward_client_call_logic", boost::bind(&client_call_gate::forward_client_call_logic, this, _1)));
        protcolcall_set.insert(std::make_pair("heartbeats", boost::bind(&client_call_gate::heartbeats, this, _1)));
    }

    ~client_call_gate(){
    }

    boost::signals2::signal<void(std::string) > sigconnect_serverhandle;
    void connect_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigconnect_serverhandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sigcancle_serverhandle;
    void cancle_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigcancle_serverhandle(
);
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > sigforward_client_call_logichandle;
    void forward_client_call_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sigforward_client_call_logichandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

    boost::signals2::signal<void() > sigheartbeatshandle;
    void heartbeats(boost::shared_ptr<std::vector<boost::any> > _event){
        sigheartbeatshandle(
);
    }

};

}
