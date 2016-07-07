/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
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
    void test_func(boost::shared_ptr<std::vector<boost::any> > _event){
        sigtest_funchandle(
            boost::any_cast<std::string>((*_event)[0]), 
            boost::any_cast<int64_t>((*_event)[1]));
    }

};

}
