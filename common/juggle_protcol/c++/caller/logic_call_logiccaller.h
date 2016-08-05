/*this caller file is codegen by juggle for c++*/
#ifndef _logic_call_logic_caller_h
#define _logic_call_logic_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class logic_call_logic : public juggle::Icaller {
public:
    logic_call_logic(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "logic_call_logic";
    }

    ~logic_call_logic(){
    }

    void reg_logic(std::string argv0,std::string argv1){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("logic_call_logic");
        v->push_back("reg_logic");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void ack_reg_logic(std::string argv0,std::string argv1){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("logic_call_logic");
        v->push_back("ack_reg_logic");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void logic_call_logic_mothed(std::string argv0,std::string argv1,std::shared_ptr<std::vector<boost::any> > argv2){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("logic_call_logic");
        v->push_back("logic_call_logic_mothed");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

};

}

#endif
