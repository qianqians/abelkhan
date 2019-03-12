/*this caller file is codegen by juggle for c++*/
#ifndef _client_call_hub_caller_h
#define _client_call_hub_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include "Icaller.h"
#include "Ichannel.h"
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class client_call_hub : public juggle::Icaller {
public:
    client_call_hub(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "client_call_hub";
    }

    ~client_call_hub(){
    }

    void client_connect(std::string argv0){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("client_call_hub");
        v->push_back("client_connect");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void call_hub(std::string argv0,std::string argv1,std::string argv2,std::shared_ptr<std::vector<boost::any> > argv3){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("client_call_hub");
        v->push_back("call_hub");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

};

}

#endif
