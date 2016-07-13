/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_logic_module_h
#define _hub_call_logic_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class hub_call_logic : public juggle::Imodule {
public:
    hub_call_logic(){
        module_name = "hub_call_logic";
        protcolcall_set.insert(std::make_pair("reg_logic_sucess", boost::bind(&hub_call_logic::reg_logic_sucess, this, _1)));
        protcolcall_set.insert(std::make_pair("hub_call_logic", boost::bind(&hub_call_logic::hub_call_logic, this, _1)));
    }

    ~hub_call_logic(){
    }

    boost::signals2::signal<void() > sigreg_logic_sucesshandle;
    void reg_logic_sucess(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logic_sucesshandle(
);
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > sighub_call_logichandle;
    void hub_call_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sighub_call_logichandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

};

}

#endif
