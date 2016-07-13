/*this caller file is codegen by juggle for c++*/
#ifndef _gate_call_client_caller_h
#define _gate_call_client_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class gate_call_client : public juggle::Icaller {
public:
    gate_call_client(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "gate_call_client";
    }

    ~gate_call_client(){
    }

    void ack_connect_server(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
        v->push_back("ack_connect_server");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void call_client(std::string argv0,std::string argv1,std::string argv2){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
        v->push_back("call_client");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

};

}

#endif
