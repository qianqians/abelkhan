/*this module file is codegen by juggle for c++*/
#ifndef _hub_module_h
#define _hub_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class hub : public juggle::Imodule {
public:
    hub(){
        module_name = "hub";
        protcolcall_set.insert(std::make_pair("reg_logic", boost::bind(&hub::reg_logic, this, _1)));
        protcolcall_set.insert(std::make_pair("logic_call_hub_mothed", boost::bind(&hub::logic_call_hub_mothed, this, _1)));
    }

    ~hub(){
    }

    boost::signals2::signal<void(std::string) > sigreg_logichandle;
    void reg_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logichandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > siglogic_call_hub_mothedhandle;
    void logic_call_hub_mothed(boost::shared_ptr<std::vector<boost::any> > _event){
        siglogic_call_hub_mothedhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

};

}

#endif
