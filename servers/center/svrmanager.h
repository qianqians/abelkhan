/*
 * svrmanager.h
 *
 *  Created on: 2016-7-9
 *      Author: qianqians
 */
#ifndef _svrmanager_h
#define _svrmanager_h

#include <list>
#include <unordered_map>
#include <tuple>
#include <map>

#include <Ichannel.h>

namespace center {

class svrmanager {
public:
	svrmanager() {
	}

	~svrmanager(){
	}

	void reg_channel(std::shared_ptr<juggle::Ichannel> ch, std::string type, std::string ip, int64_t port, std::string uuid) {
		svr_channel_info.insert(std::make_pair(ch, std::make_tuple(type, ip, port, uuid)));
		svr_info.insert(std::make_pair(uuid, std::make_tuple(type, ip, port, uuid)));
		svr_list.push_back(ch);

		if (type == "dbproxy") {
			if (dbproxy_info.size() > 0) {
				std::cout << "multi dbproxy connect svr_id:" << uuid << "ip:" << ip << "port" << port << std::endl;
			}
			dbproxy_info.push_back(std::make_tuple(type, ip, port, uuid));
		}

		if (type == "hub") {
			hub_list.push_back(ch);
		}
	}

	void unreg_channel(std::shared_ptr<juggle::Ichannel> ch) {
		for (auto it = svr_list.begin(); it != svr_list.end(); it++) {
			if (*it == ch) {
				svr_list.erase(it);
				break;
			}
		}

		auto f = svr_channel_info.find(ch);
		if (f != svr_channel_info.end()) {
			auto uuid = std::get<3>(f->second);
			svr_info.erase(uuid);
		}
		svr_channel_info.erase(ch);
	}

	bool is_svr(std::shared_ptr<juggle::Ichannel> ch) {
		for (auto it = svr_list.begin(); it != svr_list.end(); it++) {
			if (*it == ch) {
				return true;
			}
		}

		return false;
	}

	bool get_svr_info(std::tuple<std::string, std::string, int64_t, std::string> & info, std::shared_ptr<juggle::Ichannel> ch) {
		auto f = svr_channel_info.find(ch);
		if (f == svr_channel_info.end()) {
			return false;
		}

		info = f->second;

		return true;
	}

	bool get_svr_info(std::tuple<std::string, std::string, int64_t, std::string> & info, std::string uuid) {
		auto f = svr_info.find(uuid);
		if (f == svr_info.end()) {
			return false;
		}

		info = f->second;

		return true;
	}

	bool get_dbproxy_info(std::tuple<std::string, std::string, int64_t, std::string> & info) {
		if (dbproxy_info.size() <= 0) {
			return false;
		}

		info = dbproxy_info[0];

		return true;
	}

	void for_each_svr(std::function<void(std::shared_ptr<juggle::Ichannel> ch)> fn) {
		for (auto ch : svr_list) {
			fn(ch);
		}
	}

	void for_each_hub(std::function<void(std::shared_ptr<juggle::Ichannel> ch)> fn) {
		for (auto ch : hub_list) {
			fn(ch);
		}
	}

private:
	std::list<std::shared_ptr<juggle::Ichannel> > hub_list;

	std::list<std::shared_ptr<juggle::Ichannel> > svr_list;

	std::map<std::shared_ptr<juggle::Ichannel>, std::tuple<std::string, std::string, int64_t, std::string> > svr_channel_info;
	std::unordered_map<std::string, std::tuple<std::string, std::string, int64_t, std::string> > svr_info;

	std::vector<std::tuple<std::string, std::string, int64_t, std::string> > dbproxy_info;


};

}

#endif //_svrmanager_h
