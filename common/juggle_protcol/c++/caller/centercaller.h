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
class center : public juggle::Icaller {
public:
    center(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "center";
    }

    ~center(){
    }

    void reg_server(std::string argv0,std::string argv1,int64_t argv2,std::string argv3){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("center");
        v->push_back("reg_server");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv3);
        ch->push(v);
    }

};

}
