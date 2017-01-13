/*
 * clientmanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _clientmanager_h
#define _clientmanager_h

#include <map>
#include <memroy>

#include <Ichannel.h>
#include <connectnetworkservice.h>

#include <caller/gate_call_logiccaller.h>

#include "gateproxy.h"

namespace hub {

class gatemanager {
public:
    gatemanager(std::shared_ptr<service::connectnetworkservice> _conn) {
        conn = _conn;
	}

	~gatemanager() {
	}
    
    std::shared_ptr<gateproxy> connect_gate(std::string ip, short port, ){
        std::shared_ptr<juggle::Ichannel> ch = conn->connect(std::string ip, short port){
        
        std::shared_ptr<gateproxy> _gateproxy = std::make_shared<gateproxy>(ch);
        gate_ch_map.insert(std::make_pair(ch, _gateproxy));
        
        return _gateproxy;
    }
    
    std::shared_ptr<gateproxy> get_gateproxy(std::shared_ptr<juggle::Ichannel> gate_ch){
        if (gate_ch_map.find(gate_ch) != gate_ch_map.end()){
            return gate_ch_map[gate_ch];
        }
            
        return std::nullptr;
    }
        
    void call_group_client(std::vector<boost::any> uuids, std::string module, std::string func, std::vector<boost::any> argv){
        for (auto item : gate_ch_map){
            item.second->forward_hub_call_group_client(uuids, module, func, argv);
        }
    }
        
    void call_global_client(std::string module, std::string func, std::vector<boost::any> argv){
        for (auto item : gate_ch_map){
            item.second->forward_hub_call_global_client(module, func, argv);
        }
    }
    
private:
    std::map<std::shared_ptr<juggle::ichannel>, std::shared_ptr<gateproxy> > gate_ch_map;
    std::shared_ptr<service::connectnetworkservice> conn;

};

}

#endif //_clientmanager_h
