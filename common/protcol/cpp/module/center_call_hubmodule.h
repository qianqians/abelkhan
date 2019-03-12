/*this module file is codegen by juggle for c++*/
#ifndef _center_call_hub_module_h
#define _center_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class center_call_hub : public juggle::Imodule {
public:
    center_call_hub(){
        module_name = "center_call_hub";
        protcolcall_set.insert(std::make_pair("distribute_server_address", std::bind(&center_call_hub::distribute_server_address, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("reload", std::bind(&center_call_hub::reload, this, std::placeholders::_1)));
    }

    ~center_call_hub(){
    }

    boost::signals2::signal<void(std::string, std::string, int64_t, std::string) > sig_distribute_server_address;
    void distribute_server_address(std::shared_ptr<std::vector<boost::any> > _event){
        sig_distribute_server_address(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

    boost::signals2::signal<void(std::string) > sig_reload;
    void reload(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reload(
            boost::any_cast<std::string>((*_event)[0]));
    }

};

}

#endif
