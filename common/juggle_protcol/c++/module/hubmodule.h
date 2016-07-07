/*this module file is codegen by juggle for c++*/
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
        protcolcall_set.insert(std::make_pair("logic_call_hub", boost::bind(&hub::logic_call_hub, this, _1)));
    }

    ~hub(){
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > siglogic_call_hubhandle;
    void logic_call_hub(boost::shared_ptr<std::vector<boost::any> > _event){
        siglogic_call_hubhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

};

}
