/*
 * dbproxyproxy
 *
 *  Created on: 2016-10-12
 *      Author: qianqians
 */
#ifndef _dbproxyproxy_h
#define _dbproxyproxy_h

#include <iostream>

#include <Ichannel.h>

#include <caller/centercaller.h>

namespace hub{

class dbproxyproxy {
public:
	dbproxyproxy(std::shared_ptr<juggle::Ichannel> ch) {
		is_reg_sucess = false;
		_dbproxyproxy_ch = ch;
		_dbproxyproxy_caller = std::make_shared<caller::hub_call_dbproxy>(_center_ch);
	}

	~dbproxyproxy(){
	}
    
    void reg_hub(std::string uuid){
        std::cout << "begin connect dbproxyproxy server" << std::endl;
        _dbproxyproxy_caller->reg_hub(uuid);
    }
    
    void create_persisted_object(std::shared_ptr<std::unordered_map<std::string, boost::any> > object_info, std::string callbackid){
        _dbproxyproxy_caller->create_persisted_object(object_info, callbackid);
    }
    
    void updata_persisted_object(std::shared_ptr<std::unordered_map<std::string, boost::any> > query_json,
                                 std::shared_ptr<std::unordered_map<std::string, boost::any> > object_info, std::string callbackid){
        _dbproxyproxy_caller->updata_persisted_object(query_json, object_info, callbackid);
    }
    
    void get_object_info(std::shared_ptr<std::unordered_map<std::string, boost::any> > query_json, std::string callbackid){
        _dbproxyproxy_caller->get_object_info(query_json, callbackid);
    }

public:
	bool is_reg_sucess;

private:
	std::shared_ptr<juggle::Ichannel> _dbproxyproxy_ch;
	std::shared_ptr<caller::hub_call_dbproxy> _dbproxyproxy_caller;

};

}

#endif // !_centerproxy_h
