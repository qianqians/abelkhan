/*this module file is codegen by juggle for c++*/
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
        protcolcall_set.insert(std::make_pair("hub_call_logic", boost::bind(&hub_call_logic::hub_call_logic, this, _1)));
    }

    ~hub_call_logic(){
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
