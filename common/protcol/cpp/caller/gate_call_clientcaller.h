/*this caller file is codegen by juggle for c++*/
#ifndef _gate_call_client_caller_h
#define _gate_call_client_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include "Icaller.h"
#include "Ichannel.h"
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class gate_call_client : public juggle::Icaller {
public:
    gate_call_client(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "gate_call_client";
    }

    ~gate_call_client(){
    }

    void ntf_uuid(std::string argv0){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
        v->push_back("ntf_uuid");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void connect_gate_sucess(){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
        v->push_back("connect_gate_sucess");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

    void connect_hub_sucess(std::string argv0){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
        v->push_back("connect_hub_sucess");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void ack_heartbeats(){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
        v->push_back("ack_heartbeats");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

    void call_client(std::string argv0,std::string argv1,std::shared_ptr<std::vector<boost::any> > argv2){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_client");
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
