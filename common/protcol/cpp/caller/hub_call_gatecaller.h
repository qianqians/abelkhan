/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_gate_caller_h
#define _hub_call_gate_caller_h
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
class hub_call_gate : public juggle::Icaller {
public:
    hub_call_gate(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub_call_gate";
    }

    ~hub_call_gate(){
    }

    void reg_hub(std::string argv0,std::string argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_gate");
        v->push_back("reg_hub");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void connect_sucess(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_gate");
        v->push_back("connect_sucess");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void disconnect_client(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_gate");
        v->push_back("disconnect_client");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void forward_hub_call_client(std::string argv0,std::string argv1,std::string argv2,Fossilizid::JsonParse::JsonArray argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_gate");
        v->push_back("forward_hub_call_client");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void forward_hub_call_group_client(Fossilizid::JsonParse::JsonArray argv0,std::string argv1,std::string argv2,Fossilizid::JsonParse::JsonArray argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_gate");
        v->push_back("forward_hub_call_group_client");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void forward_hub_call_global_client(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonArray argv2){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_gate");
        v->push_back("forward_hub_call_global_client");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

};

}

#endif
