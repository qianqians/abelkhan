/*this caller file is codegen by juggle for c++*/
#ifndef _center_call_hub_caller_h
#define _center_call_hub_caller_h
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
class center_call_hub : public juggle::Icaller {
public:
    center_call_hub(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "center_call_hub";
    }

    ~center_call_hub(){
    }

    void distribute_server_address(std::string argv0,std::string argv1,int64_t argv2,std::string argv3){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("center_call_hub");
        v->push_back("distribute_server_address");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv2);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

    void reload(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("center_call_hub");
        v->push_back("reload");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

};

}

#endif
