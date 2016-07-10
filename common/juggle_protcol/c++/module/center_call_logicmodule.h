/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class center_call_logic : public juggle::Imodule {
public:
    center_call_logic(){
        module_name = "center_call_logic";
        protcolcall_set.insert(std::make_pair("ack_get_server_address", boost::bind(&center_call_logic::ack_get_server_address, this, _1)));
    }

    ~center_call_logic(){
    }

    boost::signals2::signal<void(bool, std::string, std::string, int64_t, std::string, int64_t) > sigack_get_server_addresshandle;
    void ack_get_server_address(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_get_server_addresshandle(
            boost::any_cast<bool>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<std::string>((*_event)[2]), 
            boost::any_cast<int64_t>((*_event)[3]), 
            boost::any_cast<std::string>((*_event)[4]), 
            boost::any_cast<int64_t>((*_event)[5]));
    }

};

}
