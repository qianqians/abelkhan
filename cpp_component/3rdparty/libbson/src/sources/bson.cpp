#include "bson/Value.h"

std::string BSON::Value::toBSON() const {
  std::string result;
  BSON::Value docSize;
  switch (_type) {
  case UNDEFINED:
    result = "";
    break;
  case INT32:
    result = std::string{(char *)&_int32Value, 4};
    break;
  case INT64:
    result = std::string{(char *)&_int64Value, 8};
    break;
  case DOUBLE:
    result = std::string{(char *)&_doubleValue, 8};
    break;
  case BOOL:
    result.push_back(_boolValue ? '\x01' : '\x00');
    break;
  case STRING:
    docSize = (int32)_stringValue.size() + 1;
    result = docSize.toBSON().append(_stringValue);
    result.push_back('\x00');
    break;
  case BINARY:
    docSize = (int32)_stringValue.size();
    result = docSize.toBSON();
    result.push_back('\x00');
    result.append(_stringValue);
    break;
  case DATETIME:
    result = std::string{(char *)&_datetimeValue, 8};
    break;
  case ARRAY:
    result = "";
    for (size_t i = 0; i < size(); i++) {
      const BSON::Value &val = _arrayValue[i];
      result.append(val.getTypePrefix())
          .append(std::to_string(i))
          .push_back('\x00');
      result.append(val.toBSON());
    }
    docSize = (int32)result.size() + 1 + 4;
    result = docSize.toBSON().append(result);
    result.push_back('\x00');
    break;
  case OBJECT:
    result = "";
    for (auto it = cbegin(); it != cend(); ++it) {
      const std::string name = it->first;
      const BSON::Value &val = it->second;
      result.append(val.getTypePrefix());
      result.append(name).push_back('\x00');
      result.append(val.toBSON());
    }
    docSize = (int32)result.size() + 1 + 4;
    result = docSize.toBSON().append(result);
    result.push_back('\x00');
    break;
  }
  return result;
}

std::string BSON::Value::getTypePrefix() const {
  switch (_type) {
  case UNDEFINED:
    return "\x0A";
    break;
  case INT32:
    return "\x10";
    break;
  case INT64:
    return "\x12";
    break;
  case DOUBLE:
    return "\x01";
    break;
  case BOOL:
    return "\x08";
    break;
  case STRING:
    return "\x02";
    break;
  case BINARY:
    return "\x05";
    break;
  case DATETIME:
    return "\x09";
    break;
  case ARRAY:
    return "\x04";
    break;
  case OBJECT:
    return "\x03";
    break;
  default:
    throw std::runtime_error{"unknown type"};
  }
  return "";
}

BSON::Value BSON::Value::fromBSON(const std::string &bson, BSON::Type docType) {
  BSON::Value result;
  uint32_t currentByte = 0;
  const uint8_t *docPtr = (const uint8_t *)&bson[0];
  const uint32_t docLen = *((const uint32_t *)docPtr);
  currentByte += 4;
  result.setType(docType);
  bool first = true;
  while (currentByte < docLen) {
    BSON::Value current;
    BSON::Type type = BSON::UNDEFINED;
    switch (docPtr[currentByte]) {
    case '\x0A':
      break;
    case '\x10':
      type = BSON::INT32;
      break;
    case '\x12':
      type = BSON::INT64;
      break;
    case '\x01':
      type = BSON::DOUBLE;
      break;
    case '\x08':
      type = BSON::BOOL;
      break;
    case '\x02':
      type = BSON::STRING;
      break;
    case '\x05':
      type = BSON::BINARY;
      break;
    case '\x09':
      type = BSON::DATETIME;
      break;
    case '\x04':
      type = BSON::ARRAY;
      break;
    case '\x03':
      type = BSON::OBJECT;
      break;
    case 0:
      currentByte++;
      continue;
    default:
      throw std::runtime_error{"unsupported BSON type identifier " +
                               std::to_string(docPtr[currentByte])};
    }
    current.setType(type);
    currentByte++;
    auto nameEndPos = bson.find('\x00', currentByte);
    std::string name{(const char *)(docPtr + currentByte),
                     nameEndPos - currentByte};
    currentByte += name.size() + 1;
    std::string value;
    uint32_t len;
    switch (type) {
    case BSON::UNDEFINED:
      if (docType == BSON::OBJECT)
        result[name] = current;
      else
        result.push_back(current);
      break;
    case BSON::INT32:
      if (docType == BSON::OBJECT)
        result[name] = *((BSON::int32 *)(docPtr + currentByte));
      else
        result.push_back(*((BSON::int32 *)(docPtr + currentByte)));
      currentByte += 4;
      break;
    case BSON::INT64:
      if (docType == BSON::OBJECT)
        result[name] = *((BSON::int64 *)(docPtr + currentByte));
      else
        result.push_back(*((BSON::int64 *)(docPtr + currentByte)));
      currentByte += 8;
      break;
    case BSON::DOUBLE:
      if (docType == BSON::OBJECT)
        result[name] = *((double *)(docPtr + currentByte));
      else
        result.push_back(*((double *)(docPtr + currentByte)));
      currentByte += 8;
      break;
    case BSON::BOOL:
      if (docType == BSON::OBJECT)
        result[name] = docPtr[currentByte] == '\x01';
      else
        result.push_back(docPtr[currentByte] == '\x01');
      currentByte += 1;
      break;
    case BSON::STRING:
      len = *((BSON::int32 *)(docPtr + currentByte));
      currentByte += 4;
      value = std::string{(const char *)(docPtr + currentByte), len - 1};
      currentByte += len;
      if (docType == BSON::OBJECT)
        result[name] = value;
      else
        result.push_back(value);
      break;
    case BSON::BINARY: {
      len = *((BSON::int32 *)(docPtr + currentByte));
      currentByte += 4;
      currentByte++; // binary subtype
      Value val{(const char *)(docPtr + currentByte), len};
      if (docType == BSON::OBJECT)
        result[name] = val;
      else
        result.push_back(val);
      currentByte += len;
      break;
    }
    case BSON::DATETIME:
      if (docType == BSON::OBJECT)
        result[name] =
            std::chrono::milliseconds{*((BSON::int64 *)(docPtr + currentByte))};
      else
        result.push_back(std::chrono::milliseconds{
            *((BSON::int64 *)(docPtr + currentByte))});
      currentByte += 8;
      break;
    case BSON::ARRAY:
      len = *((BSON::int32 *)(docPtr + currentByte));
      value = std::string{(const char *)(docPtr + currentByte), len};
      if (docType == BSON::OBJECT) {
        result[name] = fromBSON(value, type);
      } else {
        result.push_back(fromBSON(value, type));
      }
      currentByte += value.size();
      break;
    case BSON::OBJECT:
      len = *((BSON::int32 *)(docPtr + currentByte));
      value = std::string{(const char *)(docPtr + currentByte), len};
      if (docType == BSON::OBJECT) {
        result[name] = fromBSON(value, type);
      } else {
        result.push_back(fromBSON(value, type));
      }
      currentByte += value.size();
      break;
    }
    first = false;
  }
  return result;
}
