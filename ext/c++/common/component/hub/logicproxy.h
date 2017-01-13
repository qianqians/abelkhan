/*
 * logicproxy.h
 *
 *  Created on: 2016-10-12
 *      Author: qianqians
 */
#ifndef _logicproxy_h
#define _logicproxy_h

#include <iostream>

#include <Ichannel.h>

#include <caller/hub_call_logic.h>

namespace hub{

class logicproxy {
public:
	logicproxy(std::shared_ptr<juggle::Ichannel> ch) {
		is_reg_sucess = false;
		_logicproxy_ch = ch;
		_hub_call_logic = std::make_shared<caller::hub_call_logic>(_logicproxy_ch);
	}

	~logicproxy(){
	}

    void reg_logic_sucess_and_notify_hub_nominate(std::string hub_name){
        _hub_call_logic->reg_logic_sucess_and_notify_hub_nominate(hub_name);
    }
    
    void hub_call_logic_mothed(std::string module, std::string func, std::vector<boost::any> argv){
        if (is_reg_sucess){
            _hub_call_logic->hub_call_logic_mothed(module, func, argv);
        }
    }

private:
	std::shared_ptr<juggle::Ichannel> _logicproxy_ch;
	std::shared_ptr<caller::hub_call_logic> _hub_call_logic;

};

}

#endif // !_centerproxy_h
