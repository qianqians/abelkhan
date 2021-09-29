/*this caller file is codegen by juggle for c++*/
#ifndef _client_call_hub_caller_h
#define _client_call_hub_caller_h
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
class client_call_hub : public juggle::Icaller {
public:
    client_call_hub(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "client_call_hub";
    }

    ~client_call_hub(){
    }

    void client_connect(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_hub");
        v->push_back("client_connect");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void call_hub(std::string argv0,std::string argv1,std::string argv2,Fossilizid::JsonParse::JsonArray argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("client_call_hub");
        v->push_back("call_hub");
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
