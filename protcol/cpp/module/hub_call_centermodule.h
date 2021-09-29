/*this module file is codegen by juggle for c++*/
#ifndef _hub_call_center_module_h
#define _hub_call_center_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <JsonParse.h>
#include <string>

namespace module
{
class hub_call_center : public juggle::Imodule {
public:
    hub_call_center(){
        module_name = "hub_call_center";
        protcolcall_set.insert(std::make_pair("closed", std::bind(&hub_call_center::closed, this, std::placeholders::_1)));
    }

    ~hub_call_center(){
    }

    boost::signals2::signal<void() > sig_closed;
    void closed(Fossilizid::JsonParse::JsonArray _event){
        sig_closed();
    }

};

}

#endif
