/*this module file is codegen by juggle for c++*/
#ifndef _center_call_server_module_h
#define _center_call_server_module_h
#include <Imodule.h>
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class center_call_server : public juggle::Imodule {
public:
    center_call_server(){
        module_name = "center_call_server";
        protcolcall_set.insert(std::make_pair("reg_server_sucess", std::bind(&center_call_server::reg_server_sucess, this, std::placeholders::_1)));
        protcolcall_set.insert(std::make_pair("close_server", std::bind(&center_call_server::close_server, this, std::placeholders::_1)));
    }

    ~center_call_server(){
    }

    boost::signals2::signal<void() > sigreg_server_sucesshandle;
    void reg_server_sucess(std::shared_ptr<std::vector<boost::any> > _event){
        sigreg_server_sucesshandle(
);
    }

    boost::signals2::signal<void() > sigclose_serverhandle;
    void close_server(std::shared_ptr<std::vector<boost::any> > _event){
        sigclose_serverhandle(
);
    }

};

}

#endif
