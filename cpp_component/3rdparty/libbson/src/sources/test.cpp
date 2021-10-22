#include <bson/Value.h>
#include <iostream>

int main() {

  BSON::Value doc = BSON::Object{{"undefined", BSON::Value{}},
                                 {"int32", (BSON::int32)1},
                                 {"int64", (BSON::int64)1},
                                 {"double", 3.14},
                                 {"true", true},
                                 {"false", false},
                                 {"string", "foobar"},
                                 {"datetime", std::chrono::milliseconds{123}},
                                 {"object", BSON::Object{{"foo", "bar"}}},
                                 {"array", BSON::Array{1, 2, 3}}};

  std::string test = doc.toBSON();
  BSON::Value reDoc = BSON::Value::fromBSON(test);

  std::string &foo = reDoc["string"];
  std::cout << foo << std::endl;
  std::cout << doc.toJSON() << std::endl;
  std::cout << reDoc.toJSON() << std::endl;

  return 0;
}
