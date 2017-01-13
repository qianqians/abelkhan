/*this module file is codegen by juggle for c++*/
#ifndef _gm_center_module_h
#define _gm_center_module_h
#include <Imodule.h>
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class gm_center : public juggle::Imodule {
public:
    gm_center(){
        module_name = "gm_center";
        protcolcall_set.insert(std::make_pair("confirm_gm", std::bind(&gm_center::confirm_gm, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("close_clutter", std::bind(&gm_center::close_clutter, this, std::placeholders::_1)));
    }

    ~gm_center(){
    }

    boost::signals2::signal<void(std::string) > sigconfirm_gmhandle;
    void confirm_gm(std::shared_ptr<std::vector<boost::any> > _event){
        sigconfirm_gmhandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sigclose_clutterhandle;
    void close_clutter(std::shared_ptr<std::vector<boost::any> > _event){
        sigclose_clutterhandle(
            boost::any_cast<std::string>((*_event)[0]));
    }

};

}

#endif
