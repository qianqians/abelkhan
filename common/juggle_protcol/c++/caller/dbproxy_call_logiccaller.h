/*this caller file is codegen by juggle for c++*/
#ifndef _dbproxy_call_logic_caller_h
#define _dbproxy_call_logic_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class dbproxy_call_logic : public juggle::Icaller {
public:
    dbproxy_call_logic(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "dbproxy_call_logic";
    }

    ~dbproxy_call_logic(){
    }

    void reg_logic_sucess(){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy_call_logic");
        v->push_back("reg_logic_sucess");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

    void ack_create_persisted_object(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy_call_logic");
        v->push_back("ack_create_persisted_object");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void ack_updata_persisted_object(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy_call_logic");
        v->push_back("ack_updata_persisted_object");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void ack_get_object_info(std::string argv0,std::string argv1){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy_call_logic");
        v->push_back("ack_get_object_info");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void ack_get_object_info_end(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy_call_logic");
        v->push_back("ack_get_object_info_end");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

};

}

#endif
