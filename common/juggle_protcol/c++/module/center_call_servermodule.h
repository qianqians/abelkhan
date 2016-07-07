/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class center_call_server : public juggle::Imodule {
public:
    center_call_server(){
        module_name = "center_call_server";
        protcolcall_set.insert(std::make_pair("reg_server_sucess", boost::bind(&center_call_server::reg_server_sucess, this, _1)));
        protcolcall_set.insert(std::make_pair("close_server", boost::bind(&center_call_server::close_server, this, _1)));
    }

    ~center_call_server(){
    }

    boost::signals2::signal<void() > sigreg_server_sucesshandle;
    void reg_server_sucess(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_server_sucesshandle(
);
    }

    boost::signals2::signal<void() > sigclose_serverhandle;
    void close_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigclose_serverhandle(
);
    }

};

}
