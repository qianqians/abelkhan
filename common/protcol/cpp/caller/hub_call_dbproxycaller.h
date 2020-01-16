/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_dbproxy_caller_h
#define _hub_call_dbproxy_caller_h
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
class hub_call_dbproxy : public juggle::Icaller {
public:
    hub_call_dbproxy(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub_call_dbproxy";
    }

    ~hub_call_dbproxy(){
    }

    void reg_hub(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_dbproxy");
        v->push_back("reg_hub");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void create_persisted_object(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonTable argv2,std::string argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_dbproxy");
        v->push_back("create_persisted_object");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void updata_persisted_object(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonTable argv2,Fossilizid::JsonParse::JsonTable argv3,std::string argv4){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_dbproxy");
        v->push_back("updata_persisted_object");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv4);
        ch->push(v);
    }

    void get_object_count(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonTable argv2,std::string argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_dbproxy");
        v->push_back("get_object_count");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void get_object_info(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonTable argv2,std::string argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_dbproxy");
        v->push_back("get_object_info");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void remove_object(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonTable argv2,std::string argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_dbproxy");
        v->push_back("remove_object");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

};

}

#endif
