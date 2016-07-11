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
class dbproxy : public juggle::Icaller {
public:
    dbproxy(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {
        module_name = "dbproxy";
    }

    ~dbproxy(){
    }

    void reg_logic(std::string argv0){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy");
        v->push_back("reg_logic");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        ch->push(v);
    }

    void save_object(std::string argv0,std::string argv1,int64_t argv2){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy");
        v->push_back("save_object");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv2);
        ch->push(v);
    }

    void find_object(std::string argv0,int64_t argv1){
        auto v = boost::make_shared<std::vector<boost::any> >();
        v->push_back("dbproxy");
        v->push_back("find_object");
        v->push_back(boost::make_shared<std::vector<boost::any> >());
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv0);
        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv1);
        ch->push(v);
    }

};

}
