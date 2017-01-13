/*this module file is codegen by juggle for c++*/
#ifndef _gate_call_hub_module_h
#define _gate_call_hub_module_h
#include <Imodule.h>
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class gate_call_hub : public juggle::Imodule {
public:
    gate_call_hub(){
        module_name = "gate_call_hub";
        protcolcall_set.insert(std::make_pair("reg_hub_sucess", std::bind(&gate_call_hub::reg_hub_sucess, this, std::placeholders::_1)));
    }

    ~gate_call_hub(){
    }

    boost::signals2::signal<void() > sigreg_hub_sucesshandle;
    void reg_hub_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sigreg_hub_sucesshandle(
);
    }

};

}

#endif
