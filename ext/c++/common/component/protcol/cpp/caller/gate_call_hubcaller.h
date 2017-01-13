/*this caller file is codegen by juggle for c++*/
#ifndef _gate_call_hub_caller_h
#define _gate_call_hub_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class gate_call_hub : public juggle::Icaller {
public:
    gate_call_hub(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "gate_call_hub";
    }

    ~gate_call_hub(){
    }

    void reg_hub_sucess(){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gate_call_hub");
        v->push_back("reg_hub_sucess");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        ch->push(v);
    }

};

}

#endif
