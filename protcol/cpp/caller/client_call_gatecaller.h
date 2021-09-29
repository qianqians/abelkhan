/*this caller file is codegen by juggle for c++*/
#ifndef _client_call_gate_caller_h
#define _client_call_gate_caller_h
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
class client_call_gate : public juggle::Icaller {
public:
    client_call_gate(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "client_call_gate";
    }

    ~client_call_gate(){
    }

    void connect_server(std::string argv0,int64_t argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("connect_server");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void cancle_server(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("cancle_server");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

    void connect_hub(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("connect_hub");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void enable_heartbeats(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("enable_heartbeats");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

    void disable_heartbeats(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("disable_heartbeats");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

    void forward_client_call_hub(std::string argv0,std::string argv1,std::string argv2,Fossilizid::JsonParse::JsonArray argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("forward_client_call_hub");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void heartbeats(int64_t argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_gate");
        v->push_back("heartbeats");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

};

}

#endif
