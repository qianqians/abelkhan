/*this caller file is codegen by juggle for c++*/
#ifndef _hub_call_center_caller_h
#define _hub_call_center_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include "Icaller.h"
#include "Ichannel.h"
#include <boost/any.hpp>
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
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("hub_call_center");
        v->push_back("closed");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

};

}

#endif
