/*this module file is codegen by juggle for c++*/
#ifndef _center_call_hub_module_h
#define _center_call_hub_module_h
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class center_call_hub : public juggle::Imodule {
public:
    center_call_hub(){
        module_name = "center_call_hub";
        protcolcall_set.insert(std::make_pair("distribute_dbproxy_address", boost::bind(&center_call_hub::distribute_dbproxy_address, this, _1)));
    }

    ~center_call_hub(){
    }

    boost::signals2::signal<void(std::string, std::string, int64_t, std::string) > sigdistribute_dbproxy_addresshandle;
    void distribute_dbproxy_address(boost::shared_ptr<std::vector<boost::any> > _event){
        sigdistribute_dbproxy_addresshandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

};

}

#endif
