/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_client_caller_h
#define _hub_call_client_caller_h
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
class hub_call_client : public juggle::Icaller {
public:
    hub_call_client(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub_call_client";
    }

    ~hub_call_client(){
    }

    void call_client(std::string argv0,std::string argv1,Fossilizid::JsonParse::JsonArray argv2){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_client");
        v->push_back("call_client");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

};

}

#endif
