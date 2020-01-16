/*this caller file is codegen by juggle for c++*/
#ifndef _center_call_server_caller_h
#define _center_call_server_caller_h
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
class center_call_server : public juggle::Icaller {
public:
    center_call_server(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "center_call_server";
    }

    ~center_call_server(){
    }

    void reg_server_sucess(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("center_call_server");
        v->push_back("reg_server_sucess");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

    void close_server(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("center_call_server");
        v->push_back("close_server");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

};

}

#endif
