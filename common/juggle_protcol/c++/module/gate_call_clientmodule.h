/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class gate_call_client : public juggle::Imodule {
public:
    gate_call_client(){
        module_name = "gate_call_client";
        protcolcall_set.insert(std::make_pair("call_client", boost::bind(&gate_call_client::call_client, this, _1)));
    }

    ~gate_call_client(){
    }

    boost::signals2::signal<void(std::string, std::string, std::string) > sigcall_clienthandle;
    void call_client(boost::shared_ptr<std::vector<boost::any> > _event){
        sigcall_clienthandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]));
    }

};

}
