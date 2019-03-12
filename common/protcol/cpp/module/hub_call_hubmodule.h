/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_hub_module_h
#define _hub_call_hub_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class hub_call_hub : public juggle::Imodule {
public:
    hub_call_hub(){
        module_name = "hub_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub", std::bind(&hub_call_hub::reg_hub, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", std::bind(&hub_call_hub::reg_hub_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("hub_call_hub_mothed", std::bind(&hub_call_hub::hub_call_hub_mothed, this, std::placeholders::_1)));
    }

    ~hub_call_hub(){
    }

    boost::signals2::signal<void(std::string) > sig_reg_hub;
    void reg_hub(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_hub(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void() > sig_reg_hub_sucess;
    void reg_hub_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_hub_sucess();
    }

    boost::signals2::signal<void(std::string, std::string, std::shared_ptr<std::vector<boost::any> >) > sig_hub_call_hub_mothed;
    void hub_call_hub_mothed(std::shared_ptr<std::vector<boost::any> > _event){
        sig_hub_call_hub_mothed(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::shared_ptr<std::vector<boost::any> >>((*_event)[2]));
    }

};

}

#endif
