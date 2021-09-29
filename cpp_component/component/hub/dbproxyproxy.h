/*
 * dbproxyproxy.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _dbproxyproxy_h
#define _dbproxyproxy_h

#include <iostream>
#include <functional>

#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>

#include <absl/container/node_hash_map.h>

#include <Ichannel.h>
#include <JsonParse.h>

#include <hub_call_dbproxycaller.h>
#include <factory.h>

#include <spdlog/spdlog.h>

namespace hub{

class dbproxyproxy : public std::enable_shared_from_this<dbproxyproxy> {
public:
	dbproxyproxy(std::shared_ptr<juggle::Ichannel> ch) {
		_dbproxy_ch = ch;
		_dbproxy_caller = Fossilizid::pool::factory::create<caller::hub_call_dbproxy>(_dbproxy_ch);
	}

	~dbproxyproxy(){
	}

	class Collection {
	public:
		Collection(std::string _db, std::string _collection, std::shared_ptr<dbproxyproxy> _proxy) {
			db = _db;
			collection = _collection;
			dbproxy = _proxy;
		}

		void createPersistedObject(Fossilizid::JsonParse::JsonTable object_info, std::function<void(bool)> cb) {
			auto callbackid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
			dbproxy->_dbproxy_caller->create_persisted_object(db, collection, object_info, callbackid);
			dbproxy->create_persisted_object_callback[callbackid] = cb;
		}

		void updataPersistedObject(Fossilizid::JsonParse::JsonTable query_json, Fossilizid::JsonParse::JsonTable updata_info, std::function<void()> cb) {
			auto callbackid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
			dbproxy->_dbproxy_caller->updata_persisted_object(db, collection, query_json, updata_info, callbackid);
			dbproxy->updata_persisted_object_callback[callbackid] = cb;
		}

		void getObjectCount(Fossilizid::JsonParse::JsonTable query_json, std::function<void(uint64_t)> cb) {
			auto callbackid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
			dbproxy->_dbproxy_caller->get_object_count(db, collection, query_json, callbackid);
			dbproxy->get_object_count_callback[callbackid] = cb;
		}

		void getObjectInfo(Fossilizid::JsonParse::JsonTable query_json, std::function<void(Fossilizid::JsonParse::JsonArray)> cb, std::function<void()> end) {
			auto callbackid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
			dbproxy->_dbproxy_caller->get_object_info(db, collection, query_json, callbackid);
			dbproxy->get_object_info_callback[callbackid] = cb;
			dbproxy->get_object_info_end_callback[callbackid] = end;
		}

		void removeObject(Fossilizid::JsonParse::JsonTable query_json, std::function<void()> cb) {
			auto callbackid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
			dbproxy->_dbproxy_caller->remove_object(db, collection, query_json, callbackid);
			dbproxy->remove_object_callback[callbackid] = cb;
		}

	private:
		std::string db;
		std::string collection;
		std::shared_ptr<dbproxyproxy> dbproxy;
	};

	void reg_server(std::string uuid) {
		spdlog::trace("begin connect center server");
		_dbproxy_caller->reg_hub(uuid);
	}

	std::shared_ptr<Collection> getCollection(std::string db, std::string collection) {
		return Fossilizid::pool::factory::create<Collection>(db, collection, shared_from_this());
	}

	absl::node_hash_map<std::string, std::function<void(bool)> > create_persisted_object_callback;
	absl::node_hash_map<std::string, std::function<void()> > updata_persisted_object_callback;
	absl::node_hash_map<std::string, std::function<void(uint64_t)> > get_object_count_callback;
	absl::node_hash_map<std::string, std::function<void(Fossilizid::JsonParse::JsonArray)> > get_object_info_callback;
	absl::node_hash_map<std::string, std::function<void()> > get_object_info_end_callback;
	absl::node_hash_map<std::string, std::function<void()> > remove_object_callback;

	std::shared_ptr<juggle::Ichannel> _dbproxy_ch;
	std::shared_ptr<caller::hub_call_dbproxy> _dbproxy_caller;

};

}

#endif // !_dbproxyproxy_h
