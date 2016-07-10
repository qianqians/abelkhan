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
class logic_call_center : public juggle::Icaller {
public:
    logic_call_center(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "logic_call_center";
    }

    ~logic_call_center(){
    }

    void req_get_server_address(std::string argv0,int64_t argv1){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("logic_call_center");
        v->push_back("req_get_server_address");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}
