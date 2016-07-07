/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class routing_call_logic : public juggle::Imodule {
public:
    routing_call_logic(){
        module_name = "routing_call_logic";
        protcolcall_set.insert(std::make_pair("ack_get_object_server", boost::bind(&routing_call_logic::ack_get_object_server, this, _1)));
    }

    ~routing_call_logic(){
    }

    boost::signals2::signal<void(std::string, std::string, int64_t) > sigack_get_object_serverhandle;
    void ack_get_object_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_get_object_serverhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]));
    }

};

}
