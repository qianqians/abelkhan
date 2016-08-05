/*this caller file is codegen by juggle for c++*/
#ifndef _logic_call_center_caller_h
#define _logic_call_center_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class logic_call_center : public juggle::Icaller {
public:
    logic_call_center(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "logic_call_center";
    }

    ~logic_call_center(){
    }

    void req_get_server_address(std::string argv0,std::string argv1){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("logic_call_center");
        v->push_back("req_get_server_address");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}

#endif
