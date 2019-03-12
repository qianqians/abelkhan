/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_client_caller_h
#define _hub_call_client_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include "Icaller.h"
#include "Ichannel.h"
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class hub_call_client : public juggle::Icaller {
public:
    hub_call_client(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub_call_client";
    }

    ~hub_call_client(){
    }

    void call_client(std::string argv0,std::string argv1,std::shared_ptr<std::vector<boost::any> > argv2){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("hub_call_client");
        v->push_back("call_client");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

};

}

#endif
