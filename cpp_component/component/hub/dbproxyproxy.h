/*
 * dbproxyproxy.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _dbproxyproxy_h
#define _dbproxyproxy_h

#include <functional>

#include <crossguid/guid.hpp>

#include <spdlog/spdlog.h>
#include <bson/Value.h>

#include <abelkhan.h>

#include <modulemng_handle.h>

#include <dbproxy.h>

namespace hub{

class dbproxyproxy : public std::enable_shared_from_this<dbproxyproxy> {
public:
	dbproxyproxy(std::shared_ptr<abelkhan::Ichannel> ch) {
		_Collection = nullptr;

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

		void createPersistedObject(BSON::Value& object_info, std::function<void(EM_DB_RESULT)> cb) {
			auto _data_str = object_info.toBSON();
			std::vector<uint8_t> data;
			auto len = _data_str.size();
			data.resize(len);
			memcpy(data.data(), _data_str.data(), len);
			_dbproxy->_dbproxy_caller->create_persisted_object(_db, _collection, data)->callBack([cb]() {
				spdlog::trace("createPersistedObject sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED);
			}, [cb]() {
				spdlog::trace("createPersistedObject faild!");
				cb(EM_DB_RESULT::EM_DB_FAILD);
			})->timeout(5000, [cb]() {
				spdlog::trace("createPersistedObject timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT);
			});
		}

		void updataPersistedObject(BSON::Value& query_info, BSON::Value& updata_info, bool is_upsert, std::function<void(EM_DB_RESULT)> cb) {
			auto _query_str = query_info.toBSON();
			std::vector<uint8_t> query;
			auto len = _query_str.size();
			query.resize(len);
			memcpy(query.data(), _query_str.data(), len);

			auto _updata_str = updata_info.toBSON();
			std::vector<uint8_t> updata;
			len = _updata_str.size();
			updata.resize(len);
			memcpy(updata.data(), _updata_str.data(), len);

			_dbproxy->_dbproxy_caller->updata_persisted_object(_db, _collection, query, updata, is_upsert)->callBack([cb]() {
				spdlog::trace("updataPersistedObject sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED);
			}, [cb]() {
				spdlog::trace("updataPersistedObject faild!");
				cb(EM_DB_RESULT::EM_DB_FAILD);
			})->timeout(5000, [cb]() {
				spdlog::trace("updataPersistedObject timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT);
			});
		}

		void findAndModify(BSON::Value& query_info, BSON::Value& updata_info, bool _new, bool is_upsert, std::function<void(EM_DB_RESULT, BSON::Value)> cb) {
			auto _query_str = query_info.toBSON();
			std::vector<uint8_t> query;
			auto len = _query_str.size();
			query.resize(len);
			memcpy(query.data(), _query_str.data(), len);

			auto _updata_str = updata_info.toBSON();
			std::vector<uint8_t> updata;
			len = _updata_str.size();
			updata.resize(len);
			memcpy(updata.data(), _updata_str.data(), len);

			_dbproxy->_dbproxy_caller->find_and_modify(_db, _collection, query, updata, _new, is_upsert)->callBack([cb](std::vector<uint8_t> object_info) {
				try {
					auto doc = BSON::Value::fromBSON(std::string((const char*)object_info.data(), object_info.size()));
					if (doc.isObject()) {
						spdlog::trace("findAndModify sucessed!");
						cb(EM_DB_RESULT::EM_DB_SUCESSED, doc);
					}
					else {
						spdlog::trace("findAndModify rsp parse faild not object!");
						cb(EM_DB_RESULT::EM_DB_FAILD, BSON::Value());
					}
				}
				catch (std::runtime_error err) {
					spdlog::trace("findAndModify rsp parse faild:{0}!", err.what());
					cb(EM_DB_RESULT::EM_DB_FAILD, BSON::Value());
				}
			}, [cb]() {
				spdlog::trace("findAndModify faild!");
				cb(EM_DB_RESULT::EM_DB_FAILD, BSON::Value());
			})->timeout(5000, [cb]() {
				spdlog::trace("findAndModify timeout!");
				cb(EM_DB_RESULT::EM_DB_FAILD, BSON::Value());
			});
		}

		void getObjectCount(BSON::Value& query_info, std::function<void(EM_DB_RESULT, uint32_t)> cb) {
			auto _query_str = query_info.toBSON();
			std::vector<uint8_t> query;
			auto len = _query_str.size();
			query.resize(len);
			memcpy(query.data(), _query_str.data(), len);
			
			_dbproxy->_dbproxy_caller->get_object_count(_db, _collection, query)->callBack([cb](uint32_t count) {
				spdlog::trace("getObjectCount sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED, count);
			}, [cb]() {
				spdlog::trace("getObjectCount faild!");
				cb(EM_DB_RESULT::EM_DB_FAILD, 0);
			})->timeout(5000, [cb]() {
				spdlog::trace("getObjectCount timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT, 0);
			});
		}

		void getObjectInfo(BSON::Value& query_info, std::function<void(BSON::Array)> cb, std::function<void()> end) {
			auto _query_str = query_info.toBSON();
			std::vector<uint8_t> query;
			auto len = _query_str.size();
			query.resize(len);
			memcpy(query.data(), _query_str.data(), len);

			auto callbackid = xg::newGuid().str();
			_dbproxy->_dbproxy_caller->get_object_info(_db, _collection, query, 0, 0, "", false, callbackid);
			dbproxyproxy::get_object_info_callback[callbackid] = cb;
			dbproxyproxy::get_object_info_end_callback[callbackid] = end;
		}

		void getObjectInfoEx(BSON::Value& query_info, int32_t _skip, int32_t _limit, std::string _sort, bool _Ascending_, std::function<void(BSON::Array)> cb, std::function<void()> end) {
			auto _query_str = query_info.toBSON();
			std::vector<uint8_t> query;
			auto len = _query_str.size();
			query.resize(len);
			memcpy(query.data(), _query_str.data(), len);

			auto callbackid = xg::newGuid().str();
			_dbproxy->_dbproxy_caller->get_object_info(_db, _collection, query, _skip, _limit, _sort, _Ascending_, callbackid);
			dbproxyproxy::get_object_info_callback[callbackid] = cb;
			dbproxyproxy::get_object_info_end_callback[callbackid] = end;
		}

		void removeObject(BSON::Value& query_info, std::function<void(EM_DB_RESULT)> cb) {
			auto _query_str = query_info.toBSON();
			std::vector<uint8_t> query;
			auto len = _query_str.size();
			query.resize(len);
			memcpy(query.data(), _query_str.data(), len);

			_dbproxy->_dbproxy_caller->remove_object(_db, _collection, query)->callBack([cb]() {
				spdlog::trace("removeObject sucessed!");
				cb(EM_DB_RESULT::EM_DB_SUCESSED);
			}, [cb]() {
				spdlog::trace("removeObject faild!");
				cb(EM_DB_RESULT::EM_DB_FAILD);
			})->timeout(5000, [cb]() {
				spdlog::trace("removeObject timeout!");
				cb(EM_DB_RESULT::EM_DB_TIMEOUT);
			});
		}

	private:
		std::string _db;
		std::string _collection;
		std::shared_ptr<dbproxyproxy> _dbproxy;
	};

	concurrent::signals<void()> sig_dbproxy_init;
	void reg_server(std::string hub_name) {
		spdlog::trace("begin connect dbproxy server");
		_dbproxy_caller->reg_hub(hub_name)->callBack([this]() {
			spdlog::trace("connect dbproxy server sucessed!");
			_Collection = std::make_shared<Collection>(shared_from_this());

			sig_dbproxy_init.emit();
		}, []() {
			spdlog::trace("connect dbproxy server faild!");
		})->timeout(5000, []() {
			spdlog::trace("connect dbproxy server timeout!");
		});
	}

	std::shared_ptr<Collection> getCollection(std::string db, std::string collection) {
		_Collection->set_db_collection(db, collection);
		return _Collection;
	}

public:
	static std::unordered_map<std::string, std::function<void(BSON::Array)> > get_object_info_callback;
	static std::unordered_map<std::string, std::function<void()> > get_object_info_end_callback;

private:
	friend class Collection;

	std::shared_ptr<Collection> _Collection;
	
	std::shared_ptr<abelkhan::Ichannel> _dbproxy_ch;
	std::shared_ptr<abelkhan::hub_call_dbproxy_caller> _dbproxy_caller;

};

}

#endif // !_dbproxyproxy_h
