/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class gm_center : public juggle::Imodule {
public:
    gm_center(){
        module_name = "gm_center";
        protcolcall_set.insert(std::make_pair("close_clutter", boost::bind(&gm_center::close_clutter, this, _1)));
    }

    ~gm_center(){
    }

    boost::signals2::signal<void() > sigclose_clutterhandle;
    void close_clutter(boost::shared_ptr<std::vector<boost::any> > _event){
        sigclose_clutterhandle(
);
    }

};

}
