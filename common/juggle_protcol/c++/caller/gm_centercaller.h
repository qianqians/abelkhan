/*this caller file is codegen by juggle for c++*/
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class gm_center : public juggle::Icaller {
public:
    gm_center(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "gm_center";
    }

    ~gm_center(){
    }

    void close_clutter(){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gm_center");
        v->push_back("close_clutter");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

};

}
