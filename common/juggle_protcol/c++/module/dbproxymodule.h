/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class dbproxy : public juggle::Imodule {
public:
    dbproxy(){
        module_name = "dbproxy";
        protcolcall_set.insert(std::make_pair("save_object", boost::bind(&dbproxy::save_object, this, _1)));
        protcolcall_set.insert(std::make_pair("find_object", boost::bind(&dbproxy::find_object, this, _1)));
    }

    ~dbproxy(){
    }

    boost::signals2::signal<void(boost::shared_ptr<std::unordered_map<std::string, boost::any> >, boost::shared_ptr<std::unordered_map<std::string, boost::any> >, int64_t) > sigsave_objecthandle;
    void save_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigsave_objecthandle(
            boost::any_cast<boost::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[0]), 
            boost::any_cast<boost::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]));
    }

    boost::signals2::signal<void(boost::shared_ptr<std::unordered_map<std::string, boost::any> >, boost::shared_ptr<std::unordered_map<std::string, boost::any> >, int64_t) > sigfind_objecthandle;
    void find_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigfind_objecthandle(
            boost::any_cast<boost::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[0]), 
            boost::any_cast<boost::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[1]), 
            boost::any_cast<int64_t>((*_event)[2]));
    }

};

}
