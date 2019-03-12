/*this module file is codegen by juggle for c++*/
#ifndef _center_module_h
#define _center_module_h
#include "Imodule.h"
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class center : public juggle::Imodule {
public:
    center(){
        module_name = "center";
        protcolcall_set.insert(std::make_pair("reg_server", std::bind(&center::reg_server, this, std::placeholders::_1)));
    }

    ~center(){
    }

    boost::signals2::signal<void(std::string, std::string, int64_t, std::string) > sig_reg_server;
    void reg_server(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reg_server(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

};

}

#endif
