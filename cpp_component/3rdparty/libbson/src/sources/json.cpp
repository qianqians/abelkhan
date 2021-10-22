#include "bson/Value.h"
#include "bson/rapidjson/reader.h"

namespace BSON {

struct JsonHandler {
  std::vector<Object> objects;
  std::vector<Array> arrays;
  std::vector<Value> values;
  std::vector<std::string> keys;

  bool Null() {
    values.push_back(Value{});
    return true;
  }
  bool Bool(bool b) {
    values.push_back(b);
    return true;
  }
  bool Int(int i) {
    values.push_back(int64(i));
    return true;
  }
  bool Uint(unsigned u) {
    values.push_back(int64(u));
    return true;
  }
  bool Int64(int64_t i) {
    values.push_back(int64(i));
    return true;
  }
  bool Uint64(uint64_t u) {
    values.push_back(int64(u));
    return true;
  }
  bool Double(double d) {
    values.push_back(d);
    return true;
  }
  bool String(const char *str, rapidjson::SizeType length, bool copy) {
    values.push_back(std::string{str, length});
    return true;
  }
  bool StartObject() {
    objects.push_back(Object{});
    return true;
  }
  bool Key(const char *str, rapidjson::SizeType length, bool copy) {
    keys.push_back(std::string{str, length});
    return true;
  }
  bool EndObject(rapidjson::SizeType memberCount) {
    for (size_t i = 0; i < memberCount; i++) {
      objects.back()[keys.back()] = values.back();
      keys.pop_back();
      values.pop_back();
    }
    values.push_back(objects.back());
    objects.pop_back();
    return true;
  }
  bool StartArray() {
    arrays.push_back(Array{});
    return true;
  }
  bool EndArray(rapidjson::SizeType elementCount) {
    for (size_t i = 0; i < elementCount; i++) {
      arrays.back().push_back(values[values.size() - elementCount + i]);
    }
    values.erase(values.begin() + (values.size() - elementCount), values.end());
    values.push_back(arrays.back());
    arrays.pop_back();
    return true;
  }
};

std::string escapeJSON(const std::string &input) {
  std::string output{};
  output.reserve(input.length());

  for (std::string::size_type i = 0; i < input.length(); ++i) {
    switch (input[i]) {
    case '"':
      output += "\\\"";
      break;
    case '/':
      output += "\\/";
      break;
    case '\b':
      output += "\\b";
      break;
    case '\f':
      output += "\\f";
      break;
    case '\n':
      output += "\\n";
      break;
    case '\r':
      output += "\\r";
      break;
    case '\t':
      output += "\\t";
      break;
    case '\\':
      output += "\\\\";
      break;
    default:
      output += input[i];
      break;
    }
  }
  return output;
}

Value Value::fromJSON(const std::string &str) {
  rapidjson::Reader reader;
  rapidjson::StringStream ss{str.c_str()};
  JsonHandler handler;
  reader.Parse(ss, handler);
  if (handler.values.size() == 1) {
    return handler.values[0];
  }
  return Value{};
}

std::string Value::toJSON() const {
  std::string result;
  switch (_type) {
  case UNDEFINED:
    result = "null";
    break;
  case INT32:
    result = std::to_string(_int32Value);
    break;
  case INT64:
    result = std::to_string(_int64Value);
    break;
  case DOUBLE:
    result = std::to_string(_doubleValue);
    break;
  case BOOL:
    result = _boolValue ? "true" : "false";
    break;
  case STRING:
    result = "\"" + escapeJSON(_stringValue) + "\"";
    break;
  case BINARY:
    throw std::runtime_error{"BINARY data type is not representable in json"};
    break;
  case DATETIME:
    result = std::to_string(_datetimeValue.count());
    break;
  case ARRAY:
    result = "[";
    if (this->size())
      for (size_t i = 0; i <= size() - 1; ++i) {
        result += _arrayValue[i].toJSON();
        if (i < size() - 1) {
          result += ",";
        }
      }
    result += "]";
    break;
  case OBJECT:
    result += "{";
    size_t current = 0;
    for (auto &kv : _objectValue) {
      result += "\"";
      result += escapeJSON(kv.first);
      result += "\":";
      result += kv.second.toJSON();
      if (current++ < size() - 1) {
        result += ",";
      }
    }
    result += "}";
    break;
  }
  return result;
}
}
