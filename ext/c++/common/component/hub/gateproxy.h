/*
 * centerproxy.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _centerproxy_h
#define _centerproxy_h

#include <iostream>

#include <Ichannel.h>

#include <caller/hub_call_gatecaller.h>

namespace hub{

class gateproxy {
public:
	gateproxy(std::shared_ptr<juggle::Ichannel> ch) {
		is_reg_sucess = false;
		_gate_ch = ch;
		_hub_call_gatecaller = std::make_shared<caller::hub_call_gatecaller>(_gate_ch);
	}

	~gateproxy(){
	}

    void reg_hub(std::string uuid){
        _hub_call_gatecaller->reg_hub(uuid);
    }
    
    void forward_hub_call_group_client(std::vector<boost::any> uuids, std::string module, std::string func, std::vector<boost::any> argv){
        if (is_reg_sucess){
            _hub_call_gatecaller->forward_hub_call_group_client(uuids, module, func, argv);
        }
    }
    
    void forward_hub_call_global_client(std::string module, std::string func, std::vector<boost::any> argv){
        if (is_reg_sucess){
            _hub_call_gatecaller->forward_hub_call_global_client(uuids, module, func, argv);
        }
    }

public:
	bool is_reg_sucess;

private:
	std::shared_ptr<juggle::Ichannel> _gate_ch;
	std::shared_ptr<caller::hub_call_gatecaller> _hub_call_gatecaller;

};

}

#endif // !_centerproxy_h
