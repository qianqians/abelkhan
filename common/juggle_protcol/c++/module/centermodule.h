/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class center : public juggle::Imodule {
public:
    center(){
        module_name = "center";
        protcolcall_set.insert(std::make_pair("reg_server", boost::bind(&center::reg_server, this, _1)));
    }

    ~center(){
    }

    boost::signals2::signal<void(std::string, std::string, int64_t, std::string) > sigreg_serverhandle;
    void reg_server(boost::shared_ptr<std::vector<boost::any> > _event){
        sigreg_serverhandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<std::string>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]), 
            boost::any_cast<std::string>((*_event)[3]));
    }

};

}
