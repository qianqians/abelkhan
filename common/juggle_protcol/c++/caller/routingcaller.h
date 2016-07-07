/*this caller file is codegen by juggle for c++*/
#include <sstream>
#include <tuple>
#include <string>
#include <Icaller.h>
#include <Ichannel.h>
#include <boost/any.hpp>
#include <boost/make_shared.hpp>

namespace caller
{
class routing : public juggle::Icaller {
public:
    routing(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "routing";
    }

    ~routing(){
    }

    void req_get_object_server(std::string argv0,int64_t argv1){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("routing");
        v->push_back("req_get_object_server");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}
