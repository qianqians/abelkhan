/*
 * gate_msg_handle
 *
 *  Created on: 2016-10-12
 *      Author: qianqians
 */
#ifndef _gate_msg_handle_h
#define _gate_msg_handle_h

#include "gatemanager.h"

void reg_hub_sucess(std::shared_ptr<gatemanager> _gatemanager){
    std::shared_ptr<gateproxy> _gateproxy = _gatemanager->get_gateproxy(juggle::current_ch);
    if (_gateproxy != std::nullptr){
        _gateproxy->is_reg_sucess = false;
    }
}

#endif //_gate_msg_handle_h
