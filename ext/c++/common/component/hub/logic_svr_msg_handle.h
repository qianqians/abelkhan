/*
 * logic_svr_msg_handle.h
 *
 *  Created on: 2016-10-12
 *      Author: qianqians
 */
#ifndef _logic_svr_msg_handle_h
#define _logic_svr_msg_handle_h

#include "logicmanager.h"

void reg_logic(std::shared_ptr<logicmanager> _logicmanager, std::string uuid){
    _logicmanager->reg_logic(uuid, juggle::current_ch);
}

void logic_call_hub_mothed(std::shared_ptr<common::modulemanager> _modulemanager, std::string module, std::string func, std::vector<boost::any> argv){
    _modulemanager->process_module_mothed(module, func, argv);
}

#endif //_logic_svr_msg_handle_h
