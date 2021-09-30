/*
 * config.h
 *
 *  Created on: 2016-7-9
 *      Author: qianqians
 */
#ifndef _config_h
#define _config_h
//
//#include <string>
//#include <vector>
//#include <fstream>
//#include <exception>
//#include <memory>
//
//#include <boost/any.hpp>
//
//#include <JsonParse.h>
//
//namespace config {
//
//class config {
//public:
//	config(std::string & file) {
//		auto fs = std::ifstream(file);
//		if (!fs.is_open()) {
//			throw std::exception(("cannot find config file" + file).c_str());
//		}
//
//		std::stringstream buffer;
//		buffer << fs.rdbuf();
//		std::string buff(buffer.str());
//		fs.close();
//
//		try{ 
//			Fossilizid::JsonParse::unpacker(handle, buff);
//		}
//		catch (Fossilizid::JsonParse::jsonformatexception e) {
//			spdlog::error(buff);
//		}
//	}
//	
//	~config() {
//	}
//
//private:
//	config(std::any _handle) {
//		handle = _handle;
//	}
//
//	static void releaseconfig(config * _config) {
//		_config->~config();
//	}
//
//public:
//	bool has_key(std::string key) {
//		return (Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle))->find(key) != (Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle))->end();
//	}
//
//	bool get_value_bool(std::string key) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonBool>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
//	}
//
//	int64_t get_value_int(std::string key) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonInt>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
//	}
//
//	double get_value_float(std::string key) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonFloat>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
//	}
//
//	std::string get_value_string(std::string key) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonString>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key]);
//	}
//
//	std::shared_ptr<config> get_value_dict(std::string key) {
//		auto _handle = (*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key];
//		config * _config = new config(_handle);
//
//		return std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1));
//	}
//
//	std::shared_ptr<config> get_value_list(std::string key) {
//		auto _handle = (*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle)))[key];
//		config * _config = new config(_handle);
//
//		return std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1));
//	}
//
//	void for_each_value(std::function<void(std::string key, std::shared_ptr<config> value)> fn) {
//		auto table = (Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonTable>(handle));
//		for (auto it : *table) {
//			config * _config = new config(it.second);
//			fn(it.first, std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1)));
//		}
//	}
//
//	size_t get_list_size() {
//		return (Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle))->size();
//	}
//
//	bool get_list_bool(int index) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonBool>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
//	}
//
//	int64_t get_list_int(int index) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonInt>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
//	}
//
//	double get_list_float(int index) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonFloat>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
//	}
//
//	std::string get_list_string(int index) {
//		return Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonString>((*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index]);
//	}
//
//	std::shared_ptr<config> get_list_dict(int index) {
//		auto _handle = (*(Fossilizid::JsonParse::JsonCast<Fossilizid::JsonParse::JsonArray>(handle)))[index];
//		config * _config = new config(_handle);
//
//		return std::shared_ptr<config>(_config, std::bind(config::releaseconfig, std::placeholders::_1));
//	}
//
//private:
//	std::any handle;
//
//};
//
//} /* namespace config */

#endif //_config_h
