/*
 * mongodbproxy.cpp
 * qianqians
 * 2024/5/30
 */
#include <spdlog/spdlog.h>

#include "mongodbproxy.h"

namespace dbproxy
{

mongodbproxy::mongodbproxy(std::string url) {
	mongoc_init();

	bson_error_t error;
	uri = mongoc_uri_new_with_error(url.c_str(), &error);
	if (!uri) {
		spdlog::error("mongoc_uri_new_with_error error:{0}!", error.message);
		abort();
	}

	client = mongoc_client_new_from_uri_with_error(uri, &error);
	if (!client) {
		spdlog::error("mongoc_client_new_from_uri error:{0}!", error.message);
		abort();
	}

	mongoc_client_set_error_api(client, 2);
}

mongodbproxy::~mongodbproxy() {
	mongoc_uri_destroy(uri);
	mongoc_client_destroy(client);
	mongoc_cleanup();
}

void mongodbproxy::create_index(std::string db, std::string collection, std::string key, bool is_unique) {
	try {
		bson_error_t error;

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _k = BCON_NEW(key.c_str(), BCON_INT32(1));
		auto _opt = BCON_NEW("unique", BCON_BOOL(is_unique));
		mongoc_index_model_t* im = mongoc_index_model_new(_k, _opt);
		if (!mongoc_collection_create_indexes_with_opts(_collection, &im, 1, nullptr, nullptr, &error)) {
			spdlog::error("mongoc_collection_create_indexes_with_opts error:{0}", error.message);
		}

		bson_destroy(_k);
		bson_destroy(_opt);
		mongoc_collection_destroy(_collection);
	}
	catch (std::exception ex) {
		spdlog::error("create_index error:{0}", ex.what());
	}
}

void mongodbproxy::check_int_guid(std::string db, std::string collection, int64_t _guid) {
	try {
		bson_error_t error;

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _k = BCON_NEW("Guid", BCON_UTF8("__guid__"));
		auto _c = mongoc_collection_find_with_opts(_collection, _k, nullptr, nullptr);

		if (mongoc_cursor_error(_c, &error)) {
			spdlog::error("mongoc_collection_find_with_opts error:{0}", error.message);

			auto _d = BCON_NEW("Guid", BCON_UTF8("__guid__"), "inside_guid", BCON_INT64(_guid));
			if (!mongoc_collection_insert_one(_collection, _d, NULL, NULL, &error)) {
				spdlog::error("mongoc_collection_insert_one error:{0}", error.message);
			}
			bson_destroy(_d);
		}

		mongoc_cursor_destroy(_c);
		bson_destroy(_k);
		mongoc_collection_destroy(_collection);
	}
	catch (std::exception ex) {
		spdlog::error("check_int_guid error:{0}", ex.what());
	}
}

bool mongodbproxy::save(std::string db, std::string collection, std::string bson_data) {
	bool ret = true;

	try {
		bson_error_t error;

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _d = bson_new_from_data((const uint8_t *)bson_data.c_str(), bson_data.length());
		if (!mongoc_collection_insert_one(_collection, _d, NULL, NULL, &error)) {
			spdlog::error("mongoc_collection_insert_one error:{0}", error.message);
			ret = false;
		}

		bson_destroy(_d);
		mongoc_collection_destroy(_collection);
	}
	catch (std::exception ex) {
		spdlog::error("save error:{0}", ex.what());
	}

	return ret;
}

bool mongodbproxy::update(std::string db, std::string collection, std::string bson_query, std::string bson_update, bool upsert) {
	bool ret = true;

	try {
		bson_error_t error;

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _q = bson_new_from_data((const uint8_t*)bson_query.c_str(), bson_query.length());
		auto _u = bson_new_from_data((const uint8_t*)bson_update.c_str(), bson_update.length());
		auto _opt = BCON_NEW("upsert", BCON_BOOL(upsert));
		if (!mongoc_collection_update_many(_collection, _q, _u, _opt, nullptr, &error)) {
			spdlog::error("mongoc_collection_update_many error:{0}", error.message);
			ret = false;
		}

		bson_destroy(_q);
		bson_destroy(_u);
		bson_destroy(_opt);

		mongoc_collection_destroy(_collection);
	}
	catch (std::exception ex) {
		spdlog::error("update error:{0}", ex.what());
	}

	return ret;
}

bson_t* mongodbproxy::find_and_modify(std::string db, std::string collection, std::string bson_query, std::string bson_update, bool _new, bool _upsert) {
	try {
		bson_error_t error;
		bson_t* reply = BCON_NEW();

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _q = bson_new_from_data((const uint8_t*)bson_query.c_str(), bson_query.length());
		auto _u = bson_new_from_data((const uint8_t*)bson_update.c_str(), bson_update.length());
		if (!mongoc_collection_find_and_modify(_collection, _q, nullptr, _u, nullptr, false, _upsert, _new, reply, &error)) {
			spdlog::error("mongoc_collection_find_and_modify error:{0}", error.message);
		}

		bson_destroy(_q);
		bson_destroy(_u);

		mongoc_collection_destroy(_collection);

		return reply;
	}
	catch (std::exception ex) {
		spdlog::error("find_and_modify error:{0}", ex.what());
	}

	return nullptr;
}

mongoc_cursor_t* mongodbproxy::find(std::string db, std::string collection, std::string bson_query, int skip, int limit, std::string sort, bool _Ascending) {
	try {
		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _q = bson_new_from_data((const uint8_t*)bson_query.c_str(), bson_query.length());
		auto opts = BCON_NEW("sort", "{", sort.c_str(), BCON_INT32(_Ascending ? 1 : 0), "}", "skip", BCON_INT32(skip), "limit", BCON_INT32(limit));
		auto _c = mongoc_collection_find_with_opts(_collection, _q, opts, nullptr);

		bson_destroy(_q);
		mongoc_collection_destroy(_collection);

		return _c;
	}
	catch (std::exception ex) {
		spdlog::error("find error:{0}", ex.what());
	}

	return nullptr;
}

int mongodbproxy::count(std::string db, std::string collection, std::string bson_query) {
	try {
		bson_error_t error;

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _q = bson_new_from_data((const uint8_t*)bson_query.c_str(), bson_query.length());
		auto _c = (int)mongoc_collection_count_documents(_collection, _q, nullptr, nullptr, nullptr, &error);
		if (_c < 0) {
			spdlog::error("mongoc_collection_count_documents error:{0}", error.message);
		}

		bson_destroy(_q);
		mongoc_collection_destroy(_collection);

		return _c;
	}
	catch (std::exception ex) {
		spdlog::error("count error:{0}", ex.what());
	}

	return 0;
}

bool mongodbproxy::remove(std::string db, std::string collection, std::string bson_query) {
	try {
		bson_error_t error;

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _q = bson_new_from_data((const uint8_t*)bson_query.c_str(), bson_query.length());
		auto _b = mongoc_collection_delete_many(_collection, _q, nullptr, nullptr, &error);
		if (!_b) {
			spdlog::error("mongoc_collection_delete_many error:{0}", error.message);
		}

		bson_destroy(_q);
		mongoc_collection_destroy(_collection);

		return _b;
	}
	catch (std::exception ex) {
		spdlog::error("remove error:{0}", ex.what());
	}

	return 0;
}

int64_t mongodbproxy::get_guid(std::string db, std::string collection) {
	try {
		bson_error_t error;
		bson_t* reply = BCON_NEW();

		auto _collection = mongoc_client_get_collection(client, db.c_str(), collection.c_str());

		auto _q = BCON_NEW("Guid", BCON_UTF8("__guid__"));
		auto _u = BCON_NEW("$inc", "{", "inside_guid", BCON_INT32(1), "}");
		if (!mongoc_collection_find_and_modify(_collection, _q, nullptr, _u, nullptr, false, false, true, reply, &error)) {
			spdlog::error("mongoc_collection_find_and_modify error:{0}", error.message);
		}

		int64_t guid = 0;
		bson_iter_t iter;
		if (bson_iter_init(&iter, reply)) {
			auto key = std::string(bson_iter_key(&iter));
			if (key == "inside_guid") {
				guid = bson_iter_int64(&iter);
			}
		}

		bson_destroy(_q);
		bson_destroy(_u);

		mongoc_collection_destroy(_collection);

		return guid;
	}
	catch (std::exception ex) {
		spdlog::error("find_and_modify error:{0}", ex.what());
	}

	return -1;
}

}

