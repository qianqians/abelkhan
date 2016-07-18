/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_dbproxy_caller_h
#define _hub_call_dbproxy_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class hub_call_dbproxy : public juggle::Icaller {
public:
    hub_call_dbproxy(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub_call_dbproxy";
    }

    ~hub_call_dbproxy(){
    }

    void reg_hub(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("hub_call_dbproxy");
        v->push_back("reg_hub");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void create_persisted_object(std::string argv0,std::string argv1){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("hub_call_dbproxy");
        v->push_back("create_persisted_object");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void updata_persisted_object(std::string argv0,std::string argv1,std::string argv2){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("hub_call_dbproxy");
        v->push_back("updata_persisted_object");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

    void get_object_info(std::string argv0,std::string argv1){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("hub_call_dbproxy");
        v->push_back("get_object_info");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}

#endif
