/*this module file is codegen by juggle for c++*/
#ifndef _gm_center_module_h
#define _gm_center_module_h
#include "Imodule.h"
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
        protcolcall_set.insert(std::make_pair("close_zone", std::bind(&gm_center::close_zone, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("reload", std::bind(&gm_center::reload, this, std::placeholders::_1)));
    }

    ~gm_center(){
    }

    boost::signals2::signal<void(std::string) > sig_confirm_gm;
    void confirm_gm(std::shared_ptr<std::vector<boost::any> > _event){
        sig_confirm_gm(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string) > sig_close_clutter;
    void close_clutter(std::shared_ptr<std::vector<boost::any> > _event){
        sig_close_clutter(
            boost::any_cast<std::string>((*_event)[0]));
    }

    boost::signals2::signal<void(std::string, int64_t) > sig_close_zone;
    void close_zone(std::shared_ptr<std::vector<boost::any> > _event){
        sig_close_zone(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

    boost::signals2::signal<void(std::string, std::string) > sig_reload;
    void reload(std::shared_ptr<std::vector<boost::any> > _event){
        sig_reload(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]));
    }

};

}

#endif
