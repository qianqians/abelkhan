/*
 * redismqservice.h
 *
 *  Created on: 2022-6-13
 *      Author: qianqians
 */
#ifndef _redismq_service_h
#define _redismq_service_h

#include <thread>
#include <string>

#include <hircluster.h>

namespace service {

class redismqservice {
private:
	std::thread th;
	redisClusterContext* ctx;

public:
	redismqservice(std::string redis_url, std::string password) {
		ctx = redisClusterContextInit();

	}


};

}

#endif //_clientmanager_h
