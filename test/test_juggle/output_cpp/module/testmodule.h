/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/make_shared.hpp>
#include <boost/signals2.hpp>
#include <string>
#include <Msgpack.hpp>

namespace module
{
class test : public juggle::Imodule {
public:
    test(){
        module_name = "test";
        protcolcall_set.insert(std::make_pair("test_func", boost::bind(&test::test_func, this, _1)));
    }

    ~test(){
    }

    boost::signals2::signal<void(std::string, int64_t) > sigtest_funchandle;
    void test_func(msgpack::object _event){
        sigtest_funchandle(
            std::get<0>(_event.as<std::tuple<std::string, int64_t> >()),
            std::get<1>(_event.as<std::tuple<std::string, int64_t> >()));
    }

};

}
