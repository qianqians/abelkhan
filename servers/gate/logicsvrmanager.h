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

namespace gate {

class logicsvrmanager {
public:
	logicsvrmanager() {
	}

	~logicsvrmanager(){
	}

	void reg_channel(std::string uuid, boost::shared_ptr<juggle::Ichannel> ch) {
		logic_svr_vector.push_back(std::make_pair(ch, uuid));
	}

	void unreg_channel(boost::shared_ptr<juggle::Ichannel> ch) {
		for (auto it = logic_svr_vector.begin(); it != logic_svr_vector.end(); it++){
			if (it->first == ch) {
				logic_svr_vector.erase(it);
				break;
			}
		}
	}

	boost::shared_ptr<juggle::Ichannel> get_logic() {
		srand((uint32_t)time(0));
		int random = rand() % logic_svr_vector.size();

		return logic_svr_vector[random].first;
	}

	bool is_logic(boost::shared_ptr<juggle::Ichannel> ch) {
		for (auto it = logic_svr_vector.begin(); it != logic_svr_vector.end(); it++){
			if (it->first == ch) {
				return true;
			}
		}

		return false;
	}

private:
	std::vector<std::pair<boost::shared_ptr<juggle::Ichannel>, std::string> > logic_svr_vector;

};

}

#endif //_logicsvrmanager_h
