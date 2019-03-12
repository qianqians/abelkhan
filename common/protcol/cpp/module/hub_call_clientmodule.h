/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_client_module_h
#define _hub_call_client_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class hub_call_client : public juggle::Imodule {
public:
    hub_call_client(){
        module_name = "hub_call_client";
        protcolcall_set.insert(std::make_pair("call_client", std::bind(&hub_call_client::call_client, this, std::placeholders::_1)));
    }

    ~hub_call_client(){
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
