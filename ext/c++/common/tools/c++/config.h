/*
 * config.h
 *
 *  Created on: 2016-7-9
 *      Author: qianqians
 */
#ifndef _config_h
#define _config_h

#include <string>
#include <vector>
#include <fstream>
#include <exception>
#include <memory>

#include <boost/any.hpp>

#include <JsonParser.h>

namespace config {

class config {
public:
	config(std::string & file) {
		auto fs = std::ifstream(file);
		if (!fs.is_open()) {
			throw std::exception(("cannot find config file" + file).c_str());
		}

		std::stringstream buffer;
		buffer << fs.rdbuf();
		std::string buff(buffer.str());

		Fossilizid::JsonParse::unpacker(handle, buff);
	}
	
	~config() {
	}

private:
	config(boost::any _handle) {
		handle = _handle;
	}

	static void releaseconfig(config * _config) {
		_config->~config();
	}

public:
	bool has_key(std::string key) {
		(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle))->find(key) == (Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle))->end();
	}

	bool config::get_value_bool(std::string key) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonBool>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
	}

	int64_t config::get_value_int(std::string key) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonInt>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
	}

	double config::get_value_float(std::string key) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonFloat>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
	}

	std::string config::get_value_string(std::string key) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonString>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
	}

	std::shared_ptr<config> config::get_value_dict(std::string key) {
		auto _handle = (*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key];
		config * _config = (config*)Fossilizid::pool::mempool::allocator(sizeof(config));
		new (_config) config(_handle);

		return std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1));
	}

	std::shared_ptr<config> config::get_value_list(std::string key) {
		auto _handle = (*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key];
		config * _config = (config*)Fossilizid::pool::mempool::allocator(sizeof(config));
		new (_config)config(_handle);

		return std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1));
	}

	size_t config::get_list_size() {
		return (Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle))->size();
	}

	bool config::get_list_bool(int index) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonBool>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
	}

	int64_t config::get_list_int(int index) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonInt>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
	}

	double config::get_list_float(int index) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonFloat>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
	}

	std::string config::get_list_string(int index) {
		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonString>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
	}

	std::shared_ptr<config> config::get_list_dict(int index) {
		auto _handle = (*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index];
		config * _config = (config*)Fossilizid::pool::mempool::allocator(sizeof(config));
		new (_config) config(_handle);

		return std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1));
	}

private:
	boost::any handle;

};

} /* namespace config */

#endif //_config_h
