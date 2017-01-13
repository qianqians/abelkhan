/*
 * logicsvrmanager.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _logicsvrmanager_h
#define _logicsvrmanager_h

#include <string>
#include <vector>

#include <Ichannel.h>

#include "logicproxy.h"

namespace hub {

class logicsvrmanager {
public:
	logicsvrmanager() {
	}

	~logicsvrmanager(){
	}

	void reg_logic(std::string uuid, std::shared_ptr<juggle::Ichannel> ch) {
        std::shared_ptr<logicproxy> _logicproxy = std::make_shared<logicproxy>(ch);
        logic_map.insert(std::make_pair(ch, logicproxy));
        logic_str_map.insert(std::make_pair(uuid, logicproxy));
        
        _logicproxy->reg_logic_sucess_and_notify_hub_nominate();
	}
    
    bool has_logic(std::string uuid){
        return logic_str_map.find(uuid) == logic_str_map.end();
    }
    
    std::shared_ptr<logicproxy> get_logic(std::tring uuid){
        if (has_logic(logic_uuid))
        {
            return logic_str_map[uuid];
        }
        
        return std::nullptr;
    }
    
    void call_logic_mothed(std::string logic_uuid, std::string module_name, std::string mothed_name, std::vector<boost::any> argvs){
        if (has_logic(logic_uuid))
        {
            std::shared_ptr<logicproxy> _logicproxy = get_logic(logic_uuid);
            _logicproxy->call_logic(module_name, mothed_name, argvs);
        }
    }
    
    void call_group_logic(std::vector<std::string> logic_uuids, std::string module_name, std::string mothed_name, std::vector<boost::any> argvs){
        for(std::string logic_uuid : logic_uuids)
        {
            call_logic_mothed(logic_uuid, module_name, mothed_name, argvs);
        }
    }
    
    void call_global_logic(std::string module_name, std::string mothed_name, std::vector<boost::any> argvs){
        for (auto item : logic_map){
            itemsecond->call_logic(module_name, mothed_name, argvs);
        }
    }
    
private:
    std::map<std::string, std::shared_ptr<logicproxy> > logic_str_map;
	std::map<std::shared_ptr<juggle::Ichannel>, std::shared_ptr<logicproxy> > logic_map;

};

}

#endif //_logicsvrmanager_h
