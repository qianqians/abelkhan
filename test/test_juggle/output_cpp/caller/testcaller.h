/*this caller file is codegen by juggle for c++*/
#ifndef _test_caller_h
#define _test_caller_h
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <memory>

namespace caller
{
class test : public juggle::Icaller {
public:
    test(std::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "test";
    }

    ~test(){
    }

    void test_func(std::string argv0,int64_t argv1){
        auto v = std::make_shared<std::vector<boost::any> >();
        v->push_back("test");
        v->push_back("test_func");
        v->push_back(std::make_shared<std::vector<boost::any> >());
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<std::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}

#endif
