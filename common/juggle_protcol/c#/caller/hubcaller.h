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
class hub : public juggle::Icaller {
public:
    hub(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "hub";
    }

    ~hub(){
    }

    void reg_logic(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("hub");
        v->push_back("reg_logic");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void logic_call_hub(std::string argv0,std::string argv1,std::string argv2){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("hub");
        v->push_back("logic_call_hub");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

};

}
