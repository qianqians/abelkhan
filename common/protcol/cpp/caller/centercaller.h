/*this caller file is codegen by juggle for c++*/
#ifndef _center_caller_h
#define _center_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include "Icaller.h"
#include "Ichannel.h"
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class center : public juggle::Icaller {
public:
    center(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "center";
    }

    ~center(){
    }

    void reg_server(std::string argv0,std::string argv1,int64_t argv2,std::string argv3){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("center");
        v->push_back("reg_server");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

};

}

#endif
