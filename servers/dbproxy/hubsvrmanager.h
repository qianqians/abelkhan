/*
 * hubsvrmanager.h
 *
 *  Created on: 2016-7-14
 *      Author: qianqians
 */
#ifndef _hubsvrmanager_h
#define _hubsvrmanager_h

#include <string>
#include <vector>

#include <Ichannel.h>

namespace dbproxy {

class hubsvrmanager {
public:
	hubsvrmanager() {
	}

	~hubsvrmanager(){
	}

	void reg_channel(std::string uuid, std::shared_ptr<juggle::Ichannel> ch) {
		hub_svr_vector.push_back(std::make_pair(ch, uuid));
	}

	void unreg_channel(std::shared_ptr<juggle::Ichannel> ch) {
		for (auto it = hub_svr_vector.begin(); it != hub_svr_vector.end(); it++){
			if (it->first == ch) {
				hub_svr_vector.erase(it);
				break;
			}
		}
	}

	bool is_hub(std::shared_ptr<juggle::Ichannel> ch) {
		for (auto it = hub_svr_vector.begin(); it != hub_svr_vector.end(); it++){
			if (it->first == ch) {
				return true;
			}
		}

		return false;
	}

private:
	std::vector<std::pair<std::shared_ptr<juggle::Ichannel>, std::string> > hub_svr_vector;

};

}

#endif //_hubsvrmanager_h
