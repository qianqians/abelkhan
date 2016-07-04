/*this caller file is codegen by juggle for c++*/
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <MsgPack.hpp>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class test : public juggle::Icaller {
public:
    test(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "test";
    }

    ~test(){
    }

    void test_func(std::string argv0,int64_t argv1){
        std::tuple<std::string, std::string, std::tuple<std::stringint64_t> _argv(module_name, "test_func", std::make_tuple(argv0argv1));
        std::stringstream ss;
        msgpack::pack(ss, v);
        ch->senddata(ss.str().data(), ss.str().size());
    }

};

}
