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
#include <iostream>

#include <json11.hpp>

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
		fs.close();

		std::string err;
		handle = json11::Json::parse(buff, err);
		if (!handle.is_object() && !handle.is_array()) {
			spdlog::error(err + ":" + buff);
		}
	}
	
	virtual ~config() {
	}

	config(json11::Json _handle) {
		handle = _handle;
	}

public:
	bool has_key(std::string key) {
		return handle.object_items().find(key) != handle.object_items().end();
	}

	bool get_value_bool(std::string key) {
		return handle.object_items().find(key)->second.bool_value();
	}

	int64_t get_value_int(std::string key) {
		return handle.object_items().find(key)->second.int_value();
	}

	double get_value_float(std::string key) {
		return handle.object_items().find(key)->second.number_value();
	}

	std::string get_value_string(std::string key) {
		return handle.object_items().find(key)->second.string_value();
	}

	std::shared_ptr<config> get_value_dict(std::string key) {
		auto _handle = handle.object_items().find(key)->second;
		auto _config = std::make_shared<config>(_handle);

		return _config;
	}

	std::shared_ptr<config> get_value_list(std::string key) {
		auto _handle = handle.object_items().find(key)->second;
		auto _config = std::make_shared<config>(_handle);

		return _config;
	}

	void for_each_value(std::function<void(std::string key, std::shared_ptr<config> value)> fn) {
		auto table = handle.object_items();
		for (auto it : table) {
			auto _config = std::make_shared<config>(it.second);
			fn(it.first, _config);
		}
	}

	size_t get_list_size() {
		return handle.array_items().size();
	}

	bool get_list_bool(int index) {
		return handle.array_items()[index].bool_value();
	}

	int64_t get_list_int(int index) {
		return handle.array_items()[index].int_value();
	}

	double get_list_float(int index) {
		return handle.array_items()[index].number_value();
	}

	std::string get_list_string(int index) {
		return handle.array_items()[index].string_value();
	}

	std::shared_ptr<config> get_list_dict(int index) {
		auto _handle = handle.array_items()[index];
		auto _config = std::make_shared<config>(_handle);

		return _config;
	}

private:
	json11::Json handle;

};

} /* namespace config */

#endif //_config_h
