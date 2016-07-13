/*this caller file is codegen by juggle for c++*/
#ifndef _gate_call_logic_caller_h
#define _gate_call_logic_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class gate_call_logic : public juggle::Icaller {
public:
    gate_call_logic(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "gate_call_logic";
    }

    ~gate_call_logic(){
    }

    void reg_logic_sucess(){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_logic");
        v->push_back("reg_logic_sucess");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

    void client_connect(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_logic");
        v->push_back("client_connect");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void client_disconnect(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_logic");
        v->push_back("client_disconnect");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void client_call_logic(std::string argv0,std::string argv1,std::string argv2,std::string argv3){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_logic");
        v->push_back("client_call_logic");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

};

}

#endif
