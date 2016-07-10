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
class center_call_server : public juggle::Icaller {
public:
    center_call_server(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "center_call_server";
    }

    ~center_call_server(){
    }

    void reg_server_sucess(){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("center_call_server");
        v->push_back("reg_server_sucess");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

    void close_server(){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("center_call_server");
        v->push_back("close_server");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

};

}
