/*
 * JsonParse.h
 *
 *  Created on: 2015-7-14
 *      Author: qianqians
 */
#ifndef _JsonParse_h
#define _JsonParse_h

#include <vector>
#include <list>
#include <string>
#include <cstdint>
#include <sstream>
#include <stack>
#include <unordered_map>
#include <memory>
#include <functional>

#include <boost/any.hpp>

namespace Fossilizid{
namespace JsonParse{

class jsonformatexception : public std::exception{
public:
	jsonformatexception(char * str) : std::exception(str){
	}

};

typedef std::nullptr_t JsonNull;
typedef bool JsonBool;
typedef std::string JsonString;
typedef std::int64_t JsonInt;
typedef std::double_t JsonFloat;
typedef boost::any JsonObject;
typedef std::shared_ptr<std::unordered_map<std::string, JsonObject> > JsonTable;
JsonTable Make_JsonTable(){
	return std::make_shared<std::unordered_map<std::string, JsonObject>>();
}
typedef std::shared_ptr<std::vector<JsonObject> > JsonArray;
JsonArray Make_JsonArray(){
	return std::make_shared<std::vector<JsonObject> >();
}
static JsonNull JsonNull_t = nullptr;

template <class T>
T JsonCast(JsonObject & o){
	return boost::any_cast<T>(o);
}

std::string _pack(JsonString v){
	return v;
}

std::string _pack(JsonInt v){
	std::stringstream ss;
	ss << boost::any_cast<int64_t>(v);

	return ss.str();
}

std::string _pack(JsonFloat v){
	std::stringstream ss;
	ss << boost::any_cast<int64_t>(v);

	return ss.str();
}

std::string _pack(JsonBool v){
	if (boost::any_cast<bool>(v)){
		return "true";
	}
	return "flase";
}

std::string _pack(JsonNull v){
	return "null";
}

std::string pack(JsonTable & o);

std::string pack(JsonArray & _array);

std::string pack(JsonObject & v){
	std::string _out = "";
	if (v.type() == typeid(std::string) || v.type() == typeid(const char *) || v.type() == typeid(char *)){
		_out += "\"";
	}

	if (v.type() == typeid(std::string)){
		_out += _pack(boost::any_cast<JsonString>(v));
	}
	else if (v.type() == typeid(const char *)) {
		_out += _pack(std::string(boost::any_cast<const char *>(v)));
	}
	else if (v.type() == typeid(char *)){
		_out += _pack(std::string(boost::any_cast<char *>(v)));
	}
	else if (v.type() == typeid(bool)) {
		_out += _pack(boost::any_cast<JsonBool>(v));
	} else if (v.type() == typeid(std::int64_t)){
		_out += _pack((JsonInt)boost::any_cast<std::int64_t>(v));
	} else if (v.type() == typeid(std::int32_t)){
		_out += _pack((JsonInt)boost::any_cast<std::int32_t>(v));
	} else if (v.type() == typeid(std::uint64_t)){
		_out += _pack((JsonInt)boost::any_cast<std::uint64_t>(v));
	}else if (v.type() == typeid(std::uint32_t)){
		_out += _pack((JsonInt)boost::any_cast<std::uint32_t>(v));
	} else if (v.type() == typeid(double)){
		_out += _pack(boost::any_cast<JsonFloat>(v));
	} else if (v.type() == typeid(std::nullptr_t)){
		_out += _pack(nullptr);
	} else if (v.type() == typeid(JsonTable)){
		auto v1 = boost::any_cast<JsonTable>(v);
		_out += pack(v1);
	} else if (v.type() == typeid(JsonArray)){
		auto v1 = boost::any_cast<JsonArray>(v);
		_out += pack(v1);
	}

	if (v.type() == typeid(std::string) || v.type() == typeid(const char *) || v.type() == typeid(char *)){
		_out += "\"";
	}
	return _out;
}

std::string pack(JsonArray & _array){
	std::string _out = "[";
	if (_array->size() > 0) {
		for (auto o : *_array) {
			_out += pack(o);
			_out += ",";
		}
		_out.erase(_out.length() - 1);
	}
	_out += "]";

	return _out;
}

std::string pack(JsonTable & o){
	std::string _out = "{";
	for(auto _obj : *o){
		_out += "\"" + _pack(_obj.first) + "\"";
		_out += ":";
		_out += pack(_obj.second);
		_out += ",";
	}
	_out.erase(_out.length() - 1);
	_out += "}";

	return _out;
}

inline std::string packer(JsonTable & o){
	return pack(o);
}

int unpack(JsonObject & out, JsonString s){
	int begin = 0;
	int len = s.length();
	while (s[begin] != '[' && s[begin] != '{'){
		begin++;

		if (begin >= len){
			return 0;
		}
	}

	std::list<char> next;
	int end = 0;
	for (int i = begin; i < len; i++){
		if (s[i] == '['){
			next.push_back(']');
		}
		if (s[i] == '{'){
			next.push_back('}');
		}

		if (s[i] == next.back()){
			next.pop_back();
		}

		if (next.size() == 0){
			end = i;
			break;
		}
	}

	if (next.size() != 0){
		throw jsonformatexception("error json format: can not be determined the end");
	}

	const char * c = s.c_str() + begin;
	int type = 0;
	JsonObject obj;
	std::list<JsonObject> pre;
	len = end - begin + 1; 
	int i = 0;
	std::string key;

	for (; i < len; i++){
		if (c[i] == '{'){
			obj = Make_JsonTable();
			pre.push_back(obj);
		parsemap:
			int begin = -1, end = -1;
			
			while (1){
				if (c[i] == '}') {
					goto mapend;
				} 
				
				begin = ++i;
					
				if (c[i] == '}') {
					goto mapend;
				}
				else if (c[begin] == ' ' || c[begin] == '\0' || c[begin] == '\n' || c[begin] == '\t'){
					continue;
				} else{
					break;
				}
			}

			if (c[begin] != '\"'){
				throw jsonformatexception("error json fromat: not a conform key");
			}
			for (; i < len; ){
				if (c[++i] == '\"'){
					end = i++;
					break;
				}
			}
			if (end == -1){
				throw jsonformatexception("error json fromat: not a conform key");
			}

			key = std::string(&c[begin + 1], end - begin - 1);

			while (1){
				if (c[i] == ':' || c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
					i++;
				} else{
					break;
				}
			}

			type = 1;
			goto parse;

		mapend:
			obj = pre.back();
			pre.pop_back();

			if (pre.empty()){
				break;
			}

			i++;

			if (obj.type() == typeid(JsonTable)){
				goto parsemap;
			} else if (obj.type() == typeid(JsonArray)){
				goto parselist;
			} else{
				continue;
			}

		} else if (c[i] == '['){
			obj = Make_JsonArray();
			pre.push_back(obj);
		parselist:
			while (1){
				if (c[i] == ']') {
					goto listend;
				}

				i++;

				if (c[i] == ']') {
					goto listend;
				}
				else if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
					continue;
				} 
				else{
					break;
				}
			}

			{
				type = 2;
				goto parse;
			}

		listend:
			obj = pre.back();
			pre.pop_back();

			if (pre.empty()){
				break;
			}
			
			i++;

			if (obj.type() == typeid(JsonTable)){
				goto parsemap;
			} else if (obj.type() == typeid(JsonArray)){
				goto parselist;
			} else{
				continue;
			}
		}

	parse:
		if (type == 1){
			int vbegin = i, vend = 0, count = 0;
			if (c[vbegin] == '\"'){
				count = 1;
				i++;

				for (; i < len; i++){
					if (c[i] == '\\'){
						i++;
					}

					if (c[i] == '\"'){
						break;
					}
				}

				if (c[i] != '\"'){
					throw jsonformatexception("error json fromat error");
				}else{
					if (count != 1){
						throw jsonformatexception("error json fromat: can not be resolved value");
					}

					count = 0;
					vend = i++;

					while (1){
						if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
							i++;
							continue;
						} else{
							if ((c[i] == ',') || (c[i] == '}')){
								break;
							} else{
								throw jsonformatexception("error json fromat: can not be resolved value");
							}
						}
					}
				}
			
				if ((c[i] == ',') || (c[i] == '}')){
					boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, std::string(&c[vbegin + 1], vend - vbegin - 1)));
				}

				if (c[i] == '}') {
					goto mapend;
				} else if (c[i] == ']'){
					throw jsonformatexception("error json fromat: not a array");
				} 
			} else if ((c[i]) == 'n'){
				if (c[++i] != 'u'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'l'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'l'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}

				while (1){
					if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
						i++;
						continue;
					} else{
						if (c[i] == ','){
							break;
						} else{
							throw jsonformatexception("error json fromat: can not be resolved value");
						}
					}
				}

				if ((c[i] == ',') || (c[i] == '}')){
					boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, JsonNull_t));
				}

				if (c[i] == '}') {
					goto mapend;
				} else if (c[i] == ']'){
					throw jsonformatexception("error json fromat: not a array");
				}
			} else if ((c[i]) == 't'){
				if (c[++i] != 'r'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'u'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'e'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}

				while (1){
					if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
						i++;
						continue;
					} else{
						if (c[i] == ','){
							break;
						} else{
							throw jsonformatexception("error json fromat: can not be resolved value");
						}
					}
				}

				if ((c[i] == ',') || (c[i] == '}')){
					boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, true));
				}

				if (c[i] == '}') {
					goto mapend;
				} else if (c[i] == ']'){
					throw jsonformatexception("error json fromat: not a array");
				}
			} else if ((c[i]) == 'f'){
				if (c[++i] != 'a'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'l'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 's'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'e'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}

				while (1){
					if (c[i] == ' ' || c[i] == '\0' || c[begin] == '\n' || c[begin] == '\t'){
						i++;
						continue;
					} else{
						if (c[i] == ','){
							break;
						} else{
							throw jsonformatexception("error json fromat: can not be resolved value");
						}
					}
				}

				if ((c[i] == ',') || (c[i] == '}')){
					boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, false));
				}

				if (c[i] == '}') {
					goto mapend;
				} else if (c[i] == ']'){
					throw jsonformatexception("error json fromat: not a array");
				}
			} else if ((c[i]) == '['){
				auto _new = Make_JsonArray();
				boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, _new));
				pre.push_back(obj);
				obj = _new;
				goto parselist;
			} else if ((c[i]) == '{'){
				auto _new = Make_JsonTable();
				boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, _new));
				pre.push_back(obj);
				obj = _new;
				goto parsemap;
			} else if ((c[i]) == ',') {
			}
			else {
				bool isint = true;
				while (1){
					if ((c[i++] > '9' && c[i] < '0') && c[i] != '.' && c[i] != ' ' && c[i] != '\0' && c[i] != '\n' && c[i] != '\t'){
						throw jsonformatexception("error json fromat: can not be resolved value");
					}

					if (c[i] == '.'){
						isint = false;
						count++;
					}

					if (c[i] == ' ' && c[i] == '\0'){
						vend = i;
						while (1){
							if (c[i] == ' ' && c[i] == '\0'){
								i++;
								continue;
							} else{
								if (c[i] == ','){
									break;
								}

								throw jsonformatexception("error json fromat: can not be resolved value");
							}
						}
					}

					if (c[i] == ',' || c[i] == '}'){
						vend = i;
						break;
					}
				}
			
				std::stringstream ss;
				ss << std::string(&c[vbegin], vend - vbegin);
				if ((c[i] == ',') || (c[i] == '}')){
					if (isint){
						int64_t v;
						ss >> v;
						boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, v));
					} else{
						double v;
						ss >> v;
						boost::any_cast<JsonTable>(obj)->insert(std::make_pair(key, v));
					}
				}

				if (c[i] == '}') {
					goto mapend;
				} else if (c[i] == ']'){
					throw jsonformatexception("error json fromat: not a array");
				}
			}

			goto parsemap;

		}else if (type == 2){
			int vbegin = i, vend = 0, count = 0;
			if (c[vbegin] == '\"'){
				count = 1;
				i++;

				for (; i < len; i++){
					if (c[i] == '\\'){
						i++;
					}

					if (c[i] == '\"'){
						break;
					}
				}

				if (c[i] != '\"'){
					throw jsonformatexception("error json fromat error");
				} else{
					if (count != 1){
						throw jsonformatexception("error json fromat: can not be resolved value");
					}

					count = 0;
					vend = i++;

					while (1){
						if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
							i++;
							continue;
						} else{
							if ((c[i] == ',') || (c[i] == ']')){
								break;
							} else{
								throw jsonformatexception("error json fromat: can not be resolved value");
							}
						}
					}
				}

				if ((c[i] == ',') || (c[i] == ']')){
					boost::any_cast<JsonArray>(obj)->push_back(std::string(&c[vbegin + 1], vend - vbegin - 1));
				}

				if (c[i] == '}') {
					throw jsonformatexception("error json fromat: not a dict");
				} else if (c[i] == ']'){
					goto listend;
				}
			} else if ((c[i]) == 'n'){
				if (c[++i] != 'u'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'l'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'l'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}

				while (1){
					if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
						i++;
						continue;
					} else{
						if (c[i] == ','){
							break;
						} else{
							throw jsonformatexception("error json fromat: can not be resolved value");
						}
					}
				}

				if ((c[i] == ',') || (c[i] == ']')){
					boost::any_cast<JsonArray>(obj)->push_back(JsonNull_t);
				}

				if (c[i] == '}') {
					throw jsonformatexception("error json fromat: not a dict");
				} else if (c[i] == ']'){
					goto listend;
				}
			} else if ((c[i]) == 't'){
				if (c[++i] != 'r'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'u'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'e'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}

				while (1){
					if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
						i++;
						continue;
					} else{
						if (c[i] == ','){
							break;
						} else{
							throw jsonformatexception("error json fromat: can not be resolved value");
						}
					}
				}

				if ((c[i] == ',') || (c[i] == ']')){
					boost::any_cast<JsonArray>(obj)->push_back(true);
				}

				if (c[i] == '}') {
					throw jsonformatexception("error json fromat: not a dict");
				} else if (c[i] == ']'){
					goto listend;
				}
			} else if ((c[i]) == 'f'){
				if (c[++i] != 'a'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'l'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 's'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}
				if (c[++i] != 'e'){
					throw jsonformatexception("error json fromat: can not be resolved value");
				}

				while (1){
					if (c[i] == ' ' || c[i] == '\0' || c[i] == '\n' || c[i] == '\t'){
						i++;
						continue;
					} else{
						if (c[i] == ','){
							break;
						} else{
							throw jsonformatexception("error json fromat: can not be resolved value");
						}
					}
				}

				if ((c[i] == ',') || (c[i] == ']')){
					boost::any_cast<JsonArray>(obj)->push_back(false);
				}

				if (c[i] == '}') {
					throw jsonformatexception("error json fromat: not a dict");
				} else if (c[i] == ']'){
					goto listend;
				}
			} else if ((c[i]) == '['){
				pre.push_back(obj);
				auto _obj = Make_JsonArray();
				boost::any_cast<JsonArray>(obj)->push_back(_obj);
				obj = _obj;
				goto parselist;
			} else if ((c[i]) == '{'){
				pre.push_back(obj);
				auto _obj = Make_JsonTable();
				boost::any_cast<JsonArray>(obj)->push_back(_obj);
				obj = _obj;
				goto parsemap;
			} else if ((c[i]) == ',') {
			} else {
				bool isint = true;
				while (1){
					if ((c[i++] > '9' && c[i] < '0') && c[i] != '.' && c[i] != ' ' && c[i] != '\0' && c[i] != '\n' && c[i] != '\t'){
						throw jsonformatexception("error json fromat: can not be resolved value");
					}

					if (c[i] == '.'){
						isint = false;
						count++;
					}

					if (c[i] == ' ' && c[i] == '\0' && c[i] == '\n' && c[i] == '\t'){
						vend = i;
						while (1){
							if (c[i] == ' ' && c[i] == '\0' && c[i] == '\n' && c[i] == '\t'){
								i++;
								continue;
							} else{
								if (c[i] == ','){
									break;
								}

								throw jsonformatexception("error json fromat: can not be resolved value");
							}
						}
					}

					if (c[i] == ',' || c[i] == ']'){
						vend = i;
						break;
					}
				}

				std::stringstream ss;
				ss << std::string(&c[vbegin], vend - vbegin);
				if ((c[i] == ',') || (c[i] == ']')){
					if (isint){
						int64_t v;
						ss >> v;
						boost::any_cast<JsonArray>(obj)->push_back(v);
					} else{
						double v;
						ss >> v;
						boost::any_cast<JsonArray>(obj)->push_back(v);
					}
				}

				if (c[i] == '}') {
					throw jsonformatexception("error json fromat: not a dict");
				} else if (c[i] == ']'){
					goto listend;
				}
			}

			goto parselist;

		}
	}
	
	out = obj;

	return i;
}

inline int unpacker(JsonObject & out, JsonString s){
	return unpack(out, s);
}

} /* namespace JsonParse */
} /* namespace Fossilizid */

#endif //_routing_h
