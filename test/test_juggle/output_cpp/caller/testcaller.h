/*this caller file is codegen by juggle for c++*/
#include <sstream>
#include <tuple>
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
        std::tuple<std::string, std::string, std::tuple<std::string,int64_t> > _argv(module_name, "test_func", std::make_tuple(argv0,argv1));
        std::stringstream ss;
        msgpack::pack(ss, _argv);
        if (ss.str().size() <= 8192 - 4){
            char tmp[8192];
            size_t len = ss.str().size();
            tmp[0] = len & 0xff;
            tmp[1] = len >> 8 & 0xff;
            tmp[2] = len >> 16 & 0xff;
            tmp[3] = len >> 24 & 0xff;
            memcpy_s(&tmp[4], 8192, (char*)ss.str().data(), len);
            ch->senddata(tmp, len + 4);
        }
        else{
            size_t len = ss.str().size();
            char * tmp = new char[len + 4];
            tmp[0] = len & 0xff;
            tmp[1] = len >> 8 & 0xff;
            tmp[2] = len >> 16 & 0xff;
            tmp[3] = len >> 24 & 0xff;
            memcpy_s(&tmp[4], 8192, (char*)ss.str().data(), len);
            ch->senddata(tmp, len + 4);
        }
    }

};

}
