/*this module file is codegen by juggle for c++*/
#ifndef _logic_call_logic_module_h
#define _logic_call_logic_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class logic_call_logic : public juggle::Imodule {
public:
    logic_call_logic(){
        module_name = "logic_call_logic";
        protcolcall_set.insert(std::make_pair("reg_logic", boost::bind(&logic_call_logic::reg_logic, this, _1)));
        protcolcall_set.insert(std::make_pair("logic_call_logic_mothed", boost::bind(&logic_call_logic::logic_call_logic_mothed, this, _1)));
    }

    ~logic_call_logic(){
    }

    boost::signals2::signal<void(std::string) > sigreg_logichandle;
    void reg_logic(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_logichandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > siglogic_call_logic_mothedhandle;
    void logic_call_logic_mothed(boost::shared_ptr<std::vector<boost::any> > _event){
        siglogic_call_logic_mothedhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

};

}

#endif
