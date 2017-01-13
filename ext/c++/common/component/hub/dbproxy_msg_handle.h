/*
 * dbproxy_msg_handle.h
 *
 *  Created on: 2016-10-12
 *      Author: qianqians
 */
#ifndef _dbproxy_msg_handle_h
#define _dbproxy_msg_handle_h

#include <boost/any.hpp>

#include "dbproxyproxy.h"

void reg_hub_sucess()
{
    std::cout << "connect dbproxyproxy scussed" << std::endl;
}

void ack_create_persisted_object(std::string callbackid)
{
    
}

void ack_updata_persisted_object(std::string callbackid)
{
    
}

void ack_get_object_info(std::string callbackid, std::vector<boost::any> object_info)
{
    
}

void ack_get_object_info_end(std::string callbackid)
{
    
}

#endif //_center_msg_handle_h
