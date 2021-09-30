/*
 * hubsvrmanager.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hubsvrmanager_h
#define _hubsvrmanager_h
//
//#include <string>
//#include <map>
//
//#include <Ichannel.h>
//
//namespace gate {
//
//class hubsvrmanager {
//public:
//	hubsvrmanager() {
//	}
//
//	~hubsvrmanager(){
//	}
//
//	void reg_hub(std::string hub_name, std::shared_ptr<juggle::Ichannel> ch) {
//		hub_name_channel.insert(std::make_pair(hub_name, ch));
//		hub_channel_name.insert(std::make_pair(ch, hub_name));
//	}
//
//	std::shared_ptr<juggle::Ichannel> get_hub(std::string hub_name) {
//		if (hub_name_channel.find(hub_name) == hub_name_channel.end()){
//			return nullptr;
//		}
//		return hub_name_channel[hub_name];
//	}
//
//	std::string get_hub(std::shared_ptr<juggle::Ichannel> hub_channel) {
//		if (hub_channel_name.find(hub_channel) == hub_channel_name.end()) {
//			return "";
//		}
//		return hub_channel_name[hub_channel];
//	}
//
//	void for_each_hub(std::function<void(std::string hub_name, std::shared_ptr<juggle::Ichannel> ch)> fn){
//		for (auto it : hub_name_channel) {
//			fn(it.first, it.second);
//		}
//	}
//
//private:
//	std::map<std::string, std::shared_ptr<juggle::Ichannel> > hub_name_channel;
//	std::map<std::shared_ptr<juggle::Ichannel>, std::string> hub_channel_name;
//
//};
//
//}

#endif //_hubsvrmanager_h
