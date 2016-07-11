/*
 * mongodb_proxy.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _mongodb_proxy_h
#define _mongodb_proxy_h

#include <bcon.h>
#include <bson.h>
#include <mongoc.h>

namespace dbproxy {

class mongodb_proxy {
public:
	mongodb_proxy(std::string ip, short port, std::string db, std::string collection) {
		mongoc_init();

		mongoc_uri_t * _uri = mongoc_uri_new_for_host_port(ip.c_str(), port);
		_client = mongoc_client_new(mongoc_uri_get_string(_uri));
		mongoc_uri_destroy(_uri);

		_db = mongoc_client_get_database(_client, db.c_str());

		_collection = mongoc_database_get_collection(_db, collection.c_str());
	}

	~mongodb_proxy(){
		mongoc_collection_destroy(_collection);
		mongoc_database_destroy(_db);
		mongoc_client_destroy(_client);
		mongoc_cleanup();
	}

	bool update(std::string json_query, std::string json_update) {
		bson_error_t error;

		bson_t bquery;
		bson_init_from_json(&bquery, json_query.c_str(), json_query.length(), &error);

		bson_t bupdate;
		bson_init_from_json(&bupdate, json_update.c_str(), json_update.length(), &error);

		return mongoc_collection_update(_collection, MONGOC_UPDATE_UPSERT, &bquery, &bupdate, nullptr, &error);
	}

	std::vector<std::string> find(int skip, int limit, int batch_size, std::string json_query, std::string json_fields) {
		bson_error_t error;

		bson_t bquery;
		bson_init_from_json(&bquery, json_query.c_str(), json_query.length(), &error);

		std::vector<std::string> ret;

		if(json_fields != "") {
			bson_t bfields;
			bson_init_from_json(&bfields, json_fields.c_str(), json_fields.length(), &error);

			auto c = mongoc_collection_find(_collection, MONGOC_QUERY_NONE, skip, limit, batch_size, &bquery, &bfields, 0);

			const bson_t *doc;
			while (mongoc_cursor_next(c, &doc)) {
				auto json = bson_as_json(doc, 0);
				ret.push_back(json);
			}
		}
		else {
			auto c = mongoc_collection_find(_collection, MONGOC_QUERY_NONE, skip, limit, batch_size, &bquery, nullptr, 0);

			const bson_t *doc;
			while (mongoc_cursor_next(c, &doc)) {
				auto json = bson_as_json(doc, 0);
				ret.push_back(json);
			}
		}

		return ret;
	}

	bool remove(std::string json_query) {
		bson_error_t error;

		bson_t bquery;
		bson_init_from_json(&bquery, json_query.c_str(), json_query.length(), &error);

		return mongoc_collection_remove(_collection, MONGOC_REMOVE_NONE, &bquery, nullptr, &error);
	}

private:
	mongoc_client_t * _client;
	mongoc_database_t * _db;
	mongoc_collection_t * _collection;

};

}

#endif //_mongodb_proxy_h
