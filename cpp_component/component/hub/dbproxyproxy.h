/*
 * dbproxyproxy.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _dbproxyproxy_h
#define _dbproxyproxy_h

#include <functional>

#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>

#include <spdlog/spdlog.h>
#include <minibson.hpp>

#include <abelkhan.h>

#include <modulemng_handle.h>

#include <dbproxy.h>

namespace hub{

class dbproxyproxy : public std::enable_shared_from_this<dbproxyproxy> {
public:
	dbproxyproxy(std::shared_ptr<abelkhan::Ichannel> ch) {
		_dbproxy_ch = ch;
		_dbproxy_caller = std::make_shared<abelkhan::hub_call_dbproxy_caller>(_dbproxy_ch, service::_modulemng);
	}

	~dbproxyproxy(){
	}

	enum class EM_DB_RESULT {
		EM_DB_SUCESSED = 0,
		EM_DB_FAILD = 1,
		EM_DB_TIMEOUT = 2,
	};

	class Collection {
	private:
		friend class dbproxyproxy;

		void set_db_collection(std::string db_, std::string collection_) {
			_db = db_;
			_collection = collection_;
		}

	public:
		Collection(std::shared_ptr<dbproxyproxy> proxy_) {
			_dbproxy = proxy_;
		}

		void createPersistedObject(minibson::document& object_info, std::function<void(EM_DB_RESULT)> cb) {
			auto len = object_info.get_serialized_size();
			std::vector<uint8_t> data;
			data.resize(len);
			object_info.serialize(data.data(), len);
			_dbproxy->_dbproxy_caller->create_persisted_object(_db, _collection, data)->callBack([cb]() {
				spdlog::trace("createPersistedObject sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED);
			}, [cb]() {
				spdlog::trace("createPersistedObject failed!");
				cb(EM_DB_RESULT::EM_DB_FAILD);
			})->timeout(5 * 1000, [cb]() {
				spdlog::trace("createPersistedObject timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT);
			});
		}

		void updataPersistedObject(minibson::document& query_info, minibson::document& updata_info, std::function<void(EM_DB_RESULT)> cb) {
			auto len = query_info.get_serialized_size();
			std::vector<uint8_t> query;
			query.resize(len);
			query_info.serialize(query.data(), len);
			
			len = updata_info.get_serialized_size();
			std::vector<uint8_t> updata;
			updata.resize(len);
			updata_info.serialize(updata.data(), len);

			_dbproxy->_dbproxy_caller->updata_persisted_object(_db, _collection, query, updata)->callBack([cb]() {
				spdlog::trace("updataPersistedObject sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED);
			}, [cb]() {
				spdlog::trace("updataPersistedObject failed!");
				cb(EM_DB_RESULT::EM_DB_FAILD);
			})->timeout(5 * 1000, [cb]() {
				spdlog::trace("updataPersistedObject timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT);
			});
		}

		void findAndModify(minibson::document& query_info, minibson::document& updata_info, bool _new, std::function<void(EM_DB_RESULT, minibson::document)> cb) {
			auto len = query_info.get_serialized_size();
			std::vector<uint8_t> query;
			query.resize(len);
			query_info.serialize(query.data(), len);

			len = updata_info.get_serialized_size();
			std::vector<uint8_t> updata;
			updata.resize(len);
			updata_info.serialize(updata.data(), len);

			_dbproxy->_dbproxy_caller->find_and_modify(_db, _collection, query, updata, _new)->callBack([cb](std::vector<uint8_t> object_info) {
				spdlog::trace("findAndModify sucessed!");
				minibson::document doc(object_info.data(), object_info.size());
				cb(EM_DB_RESULT::EM_DB_SUCESSED, doc);
			}, [cb]() {
				spdlog::trace("findAndModify failed!");
				cb(EM_DB_RESULT::EM_DB_FAILD, minibson::document());
			})->timeout(5 * 1000, [cb]() {
				spdlog::trace("findAndModify timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT, minibson::document());
			});
		}

		void getObjectCount(minibson::document& query_info, std::function<void(EM_DB_RESULT, uint32_t)> cb) {
			auto len = query_info.get_serialized_size();
			std::vector<uint8_t> query;
			query.resize(len);
			query_info.serialize(query.data(), len);
			
			_dbproxy->_dbproxy_caller->get_object_count(_db, _collection, query)->callBack([cb](uint32_t count) {
				spdlog::trace("getObjectCount sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED, count);
			}, [cb]() {
				spdlog::trace("getObjectCount failed!");
				cb(EM_DB_RESULT::EM_DB_FAILD, 0);
			})->timeout(5 * 1000, [cb]() {
				spdlog::trace("getObjectCount timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT, 0);
			});
		}

		void getObjectInfo(minibson::document& query_json, std::function<void(Fossilizid::JsonParse::JsonArray)> cb, std::function<void()> end) {
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
		std::string _db;
		std::string _collection;
		std::shared_ptr<dbproxyproxy> _dbproxy;
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

	std::shared_ptr<abelkhan::Ichannel> _dbproxy_ch;
	std::shared_ptr<abelkhan::hub_call_dbproxy_caller> _dbproxy_caller;

};

}

#endif // !_dbproxyproxy_h
