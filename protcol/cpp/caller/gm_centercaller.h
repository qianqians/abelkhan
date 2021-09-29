/*this caller file is codegen by juggle for c++*/
#ifndef _gm_center_caller_h
#define _gm_center_caller_h
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
class gm_center : public juggle::Icaller {
public:
    gm_center(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "gm_center";
    }

    ~gm_center(){
    }

    void confirm_gm(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("gm_center");
        v->push_back("confirm_gm");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void close_clutter(std::string argv0){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("gm_center");
        v->push_back("close_clutter");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void close_zone(std::string argv0,int64_t argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("gm_center");
        v->push_back("close_zone");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

    void reload(std::string argv0,std::string argv1){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("gm_center");
        v->push_back("reload");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv0);
        (std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}

#endif
