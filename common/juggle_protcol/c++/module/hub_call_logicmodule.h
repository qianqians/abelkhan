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
        protcolcall_set.insert(std::make_pair("reg_logic_sucess_and_notify_hub_nominate", boost::bind(&hub_call_logic::reg_logic_sucess_and_notify_hub_nominate, this, _1)));
        protcolcall_set.insert(std::make_pair("hub_call_logic_mothed", boost::bind(&hub_call_logic::hub_call_logic_mothed, this, _1)));
    }

    ~hub_call_logic(){
    }

    boost::signals2::signal<void(std::string) > sigreg_logic_sucess_and_notify_hub_nominatehandle;
    void reg_logic_sucess_and_notify_hub_nominate(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logic_sucess_and_notify_hub_nominatehandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, boost::shared_ptr<std::vector<boost::any> >) > sighub_call_logic_mothedhandle;
    void hub_call_logic_mothed(boost::shared_ptr<std::vector<boost::any> > _event){
        sighub_call_logic_mothedhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<boost::shared_ptr<std::vector<boost::any> >>((*_event)[2]));
    }

};

}

#endif
