/*
 * mongodbproxy.h
 * qianqians
 * 2024/5/30
 */
#ifndef _mongodbproxy_h
#define _mongodbproxy_h

#include <string>
#include <mongoc/mongoc.h>

namespace dbproxy
{

class mongodbproxy
{
private:
	mongoc_client_t* client;
	mongoc_uri_t* uri;

public:
	mongodbproxy(std::string url);

	virtual ~mongodbproxy();

public:
    void create_index(std::string db, std::string collection, std::string key, bool is_unique);

    void check_int_guid(std::string db, std::string collection, int64_t _guid);
    
    bool save(std::string db, std::string collection, std::vector<uint8_t> bson_data);

    bool update(std::string db, std::string collection, std::vector<uint8_t> bson_query, std::vector<uint8_t> bson_update, bool upsert);

    bson_t* find_and_modify(std::string db, std::string collection, std::vector<uint8_t> bson_query, std::vector<uint8_t> bson_update, bool _new, bool _upsert);

    mongoc_cursor_t* find(std::string db, std::string collection, std::vector<uint8_t> bson_query, int skip, int limit, std::string sort, bool _Ascending);

    int count(std::string db, std::string collection, std::vector<uint8_t> bson_query);

    bool remove(std::string db, std::string collection, std::vector<uint8_t> bson_query);

    int64_t get_guid(std::string db, std::string collection);

};

}

#endif // !_mongodbproxy_h

