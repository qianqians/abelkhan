/*this caller file is codegen by juggle for c++*/
#ifndef _dbproxy_call_hub_caller_h
#define _dbproxy_call_hub_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include "Icaller.h"
#include "Ichannel.h"
#include <any>
#include <JsonParse.h>
#include <memory>

namespace caller
{
class dbproxy_call_hub : public juggle::Icaller {
public:
    dbproxy_call_hub(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "dbproxy_call_hub";
    }

    ~dbproxy_call_hub(){
    }

    void reg_hub_sucess(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("reg_hub_sucess");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

    void ack_create_persisted_object(std::string argv0,bool argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("ack_create_persisted_object");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void ack_updata_persisted_object(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("ack_updata_persisted_object");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void ack_get_object_count(std::string argv0,int64_t argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("ack_get_object_count");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void ack_get_object_info(std::string argv0,Fossilizid::JsonParse::JsonArray argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("ack_get_object_info");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void ack_get_object_info_end(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("ack_get_object_info_end");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void ack_remove_object(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("dbproxy_call_hub");
        v->push_back("ack_remove_object");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

};

}

#endif
