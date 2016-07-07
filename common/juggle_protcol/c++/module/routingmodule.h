/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class routing : public juggle::Imodule {
public:
    routing(){
        module_name = "routing";
        protcolcall_set.insert(std::make_pair("req_get_object_server", boost::bind(&routing::req_get_object_server, this, _1)));
    }

    ~routing(){
    }

    boost::signals2::signal<void(std::string, int64_t) > sigreq_get_object_serverhandle;
    void req_get_object_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreq_get_object_serverhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

};

}
