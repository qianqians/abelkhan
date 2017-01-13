/*this caller file is codegen by juggle for c++*/
#ifndef _gm_center_caller_h
#define _gm_center_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
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
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gm_center");
        v->push_back("confirm_gm");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void close_clutter(std::string argv0){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("gm_center");
        v->push_back("close_clutter");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

};

}

#endif
