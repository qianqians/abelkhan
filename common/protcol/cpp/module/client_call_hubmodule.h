/*this module file is codegen by juggle for c++*/
#ifndef _client_call_hub_module_h
#define _client_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class client_call_hub : public juggle::Imodule {
public:
    client_call_hub(){
        module_name = "client_call_hub";
        protcolcall_set.insert(std::make_pair("client_connect", std::bind(&client_call_hub::client_connect, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("call_hub", std::bind(&client_call_hub::call_hub, this, std::placeholders::_1)));
    }

    ~client_call_hub(){
    }

    boost::signals2::signal<void(std::string) > sig_client_connect;
    void client_connect(std::shared_ptr<std::vector<boost::any> > _event){
        sig_client_connect(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_call_hub;
    void call_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_call_hub(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[3]));
    }

};

}

#endif
