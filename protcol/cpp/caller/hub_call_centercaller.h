/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_center_caller_h
#define _hub_call_center_caller_h
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
class hub_call_center : public juggle::Icaller {
public:
    hub_call_center(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub_call_center";
    }

    ~hub_call_center(){
    }

    void closed(){
        auto v = Fossilizid::JsonParse::Make_JsonArray();
        v->push_back("hub_call_center");
        v->push_back("closed");
        v->push_back(Fossilizid::JsonParse::Make_JsonArray());
        ch->push(v);
    }

};

}

#endif
