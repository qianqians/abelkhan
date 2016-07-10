/*this module file is codegen by juggle for c++*/
#include <Imodule.h>
#include <boost/shared_ptr.hpp>
#include <boost/signals2.hpp>
#include <string>
namespace module
{
class dbproxy_call_logic : public juggle::Imodule {
public:
    dbproxy_call_logic(){
        module_name = "dbproxy_call_logic";
        protcolcall_set.insert(std::make_pair("save_ovject_sucess", boost::bind(&dbproxy_call_logic::save_ovject_sucess, this, _1)));
        protcolcall_set.insert(std::make_pair("ack_find_object", boost::bind(&dbproxy_call_logic::ack_find_object, this, _1)));
    }

    ~dbproxy_call_logic(){
    }

    boost::signals2::signal<void(int64_t) > sigsave_ovject_sucesshandle;
    void save_ovject_sucess(boost::shared_ptr<std::vector<boost::any> > _event){
        sigsave_ovject_sucesshandle(
            boost::any_cast<int64_t>((*_event)[0]));
    }

    boost::signals2::signal<void(int64_t, boost::shared_ptr<std::unordered_map<std::string, boost::any> >) > sigack_find_objecthandle;
    void ack_find_object(boost::shared_ptr<std::vector<boost::any> > _event){
        sigack_find_objecthandle(
            boost::any_cast<int64_t>((*_event)[0]), 
            boost::any_cast<boost::shared_ptr<std::unordered_map<std::string, boost::any> >>((*_event)[1]));
    }

};

}
