/*this module file is codegen by juggle for c++*/
#ifndef _test_module_h
#define _test_module_h
#include <Imodule.h>
#include <memory>
#include <boost/signals2.hpp>
#include <string>

namespace module
{
class test : public juggle::Imodule {
public:
    test(){
        module_name = "test";
        protcolcall_set.insert(std::make_pair("test_func", std::bind(&test::test_func, this, std::placeholders::_1)));
    }

    ~test(){
    }

    boost::signals2::signal<void(std::string, int64_t) > sigtest_funchandle;
    void test_func(std::shared_ptr<std::vector<boost::any> > _event){
        sigtest_funchandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

};

}

#endif
