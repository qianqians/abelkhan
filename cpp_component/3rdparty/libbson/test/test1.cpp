#include "gtest/gtest.h"
#include "bson/Value.h"

using namespace BSON;

TEST(BSON, Basic) {
  Value doc = Object{{"undefined", Value{}},
                     {"int32", (int32)1},
                     {"int64", (int64)1},
                     {"double", 3.14},
                     {"true", true},
                     {"binary", Value{"abc", 3}},
                     {"false", false},
                     {"string", "foobar"},
                     {"datetime", std::chrono::milliseconds{1704}},
                     {"object", Object{{"foo", "bar"}}},
                     {"array", Array{1, 2, 3}}};

  std::string encoded1 = doc.toBSON();
  auto doc2 = Value::fromBSON(encoded1);
  std::string encoded2 = doc2.toBSON();

  EXPECT_EQ(encoded1, encoded2);
}

TEST(BSON, Equality) {
  Value doc = Object{{"undefined", Value{}},
                     {"int32", (int32)1},
                     {"int64", (int64)1},
                     {"double", 3.14},
                     {"true", true},
                     {"binary", Value{"abc", 3}},
                     {"false", false},
                     {"string", "foobar"},
                     {"datetime", std::chrono::milliseconds{1704}},
                     {"object", Object{{"foo", "bar"}}},
                     {"array", Array{1, 2, 3}}};

  Value doc2 = 23;

  EXPECT_TRUE(doc == doc);
  EXPECT_FALSE(doc == doc2);
}

TEST(BSON, Construct) {
  // construct from native type
  Value undefined;
  Value integer_32bit{(int32_t)23};
  Value integer_64bit{(long long)42};
  Value doubleVal{3.14};
  Value boolVal{true};
  Value stringVal{"foobar"};
  Value date{std::chrono::milliseconds{1704}};
  Value object{Object{}};
  Value array{Array{}};
  EXPECT_EQ(UNDEFINED, undefined.getType());
  EXPECT_EQ(INT32, integer_32bit.getType());
  EXPECT_EQ(INT64, integer_64bit.getType());
  EXPECT_EQ(DOUBLE, doubleVal.getType());
  EXPECT_EQ(BOOL, boolVal.getType());
  EXPECT_EQ(STRING, stringVal.getType());
  EXPECT_EQ(DATETIME, date.getType());
  EXPECT_EQ(OBJECT, object.getType());
  EXPECT_EQ(ARRAY, array.getType());

  // copy constructor
  Value undefined_copy{undefined};
  Value integer_32bit_copy{integer_32bit};
  Value integer_64bit_copy{integer_64bit};
  Value doubleVal_copy{doubleVal};
  Value boolVal_copy{boolVal};
  Value stringVal_copy{stringVal};
  Value date_copy{date};
  Value object_copy{object};
  Value array_copy{array};
  EXPECT_EQ(UNDEFINED, undefined_copy.getType());
  EXPECT_EQ(INT32, integer_32bit_copy.getType());
  EXPECT_EQ(INT64, integer_64bit_copy.getType());
  EXPECT_EQ(DOUBLE, doubleVal_copy.getType());
  EXPECT_EQ(BOOL, boolVal_copy.getType());
  EXPECT_EQ(STRING, stringVal_copy.getType());
  EXPECT_EQ(DATETIME, date_copy.getType());
  EXPECT_EQ(OBJECT, object_copy.getType());
  EXPECT_EQ(ARRAY, array_copy.getType());
}

TEST(BSON, BackCasting) {
  Value integer_32bit{(int32_t)23};
  Value integer_64bit{(long long)42};
  Value doubleVal{3.14};
  Value boolVal{true};
  Value stringVal{"foobar"};
  Value date{std::chrono::milliseconds{1704}};
  Value object{Object{{"foo", "bar"}}};
  Value array{Array{1, 1, 2, 3, 5, 8, 13}};

  int32 integer_32bit_native = integer_32bit;
  int64 integer_64bit_native = integer_64bit;
  double doubleVal_native = doubleVal;
  bool boolVal_native = boolVal;
  std::string stringVal_native = stringVal;
  std::chrono::milliseconds date_native = date;
  Object object_native = object;
  Array array_native = array;

  EXPECT_EQ(23, integer_32bit_native);
  EXPECT_EQ(42, integer_64bit_native);
  EXPECT_EQ(3.14, doubleVal_native);
  EXPECT_EQ(true, boolVal_native);
  EXPECT_EQ("foobar", stringVal_native);
  EXPECT_EQ(std::chrono::milliseconds{1704}, date_native);
  EXPECT_EQ((Object{{"foo", "bar"}}), object_native);
  EXPECT_EQ((Array{1, 1, 2, 3, 5, 8, 13}), array_native);
}

TEST(BSON, JsonPrimitiveTest) {
  Value undefined;
  Value integer_32bit{(int32_t)23};
  Value integer_64bit{(long long)42};
  Value doubleVal{3.14};
  Value boolVal{true};
  Value stringVal{"foobar"};
  Value date{std::chrono::milliseconds{1704}};
  Value object{Object{{"foo", "bar"}}};
  Value array{Array{int64(1), int64(2), int64(3)}};

  Value undefined_copy{Value::fromJSON(undefined.toJSON())};
  Value integer_32bit_copy{Value::fromJSON(integer_32bit.toJSON())};
  Value integer_64bit_copy{Value::fromJSON(integer_64bit.toJSON())};
  Value doubleVal_copy{Value::fromJSON(doubleVal.toJSON())};
  Value boolVal_copy{Value::fromJSON(boolVal.toJSON())};
  Value stringVal_copy{Value::fromJSON(stringVal.toJSON())};
  Value date_copy{Value::fromJSON(date.toJSON())};
  Value object_copy{Value::fromJSON(object.toJSON())};
  Value array_copy{Value::fromJSON(array.toJSON())};

  EXPECT_TRUE(undefined == undefined_copy);
  EXPECT_TRUE(integer_64bit == integer_64bit_copy);
  EXPECT_TRUE(doubleVal == doubleVal_copy);
  EXPECT_TRUE(boolVal == boolVal_copy);
  EXPECT_TRUE(stringVal == stringVal_copy);
  EXPECT_TRUE(object == object_copy);
  EXPECT_TRUE(array == array_copy);
  // EXPECT_TRUE(integer_32bit == integer_32bit_copy);// json integers are
  // converted to int64
  // EXPECT_TRUE(date == date_copy);// date is not a supported json type.
  // backconversion ends up in a int64
}

TEST(BSON, BsonTest) {
  Value undefined;
  Value integer_32bit{(int32_t)23};
  Value integer_64bit{(long long)42};
  Value doubleVal{3.14};
  Value boolVal{true};
  Value stringVal{"foobar"};
  Value date{std::chrono::milliseconds{1704}};
  Value object{Object{}};
  Value array{Array{}};

  Value array_document =
      Array{undefined, integer_32bit, integer_64bit, doubleVal, boolVal,
            stringVal, date,          object,        array};

  Value object_document = Object{{"undefined", undefined},
                                 {"integer_32bit", integer_32bit},
                                 {"integer_64bit", integer_64bit},
                                 {"doubleVal", doubleVal},
                                 {"boolVal", boolVal},
                                 {"stringVal", stringVal},
                                 {"date", date},
                                 {"object", object},
                                 {"array", array}};

  Value reconstructed_array = Value::fromBSON(array_document.toBSON());
  Value reconstructed_object = Value::fromBSON(object_document.toBSON());

  EXPECT_EQ(array_document, reconstructed_array);
  EXPECT_EQ(object_document, reconstructed_object);
}

TEST(BSON, ConstructFromJSONPrimitiv) {
  std::string pi = "3.14";
  Value piVal = Value::fromJSON(pi);
  EXPECT_EQ(DOUBLE, piVal.getType());
  EXPECT_EQ(3.14, (double)piVal);

  std::string zero = "null";
  Value zeroVal = Value::fromJSON(zero);
  EXPECT_EQ(UNDEFINED, zeroVal.getType());
  EXPECT_EQ(Value{}, zeroVal);
}

TEST(BSON, JsonSpeed) {
  Value undefined;
  Value integer_32bit{(int32_t)23};
  Value integer_64bit{(long long)42};
  Value doubleVal{3.14};
  Value boolVal{true};
  Value stringVal{"foobar"};
  Value date{std::chrono::milliseconds{1704}};
  Value object{Object{}};
  Value array{Array{1, 2, 3}};

  Value object_document = Object{{"undefined", undefined},
                                 {"integer_32bit", integer_32bit},
                                 {"integer_64bit", integer_64bit},
                                 {"doubleVal", doubleVal},
                                 {"boolVal", boolVal},
                                 {"stringVal", stringVal},
                                 {"date", date},
                                 {"object", object},
                                 {"array", array}};
  std::string str;
  for (int i = 0; i < 100000; i++) {
    str = object_document.toJSON();
    object_document = Value::fromJSON(str);
  }
}

TEST(BSON, BsonSpeed) {
  Value undefined;
  Value integer_32bit{(int32_t)23};
  Value integer_64bit{(long long)42};
  Value doubleVal{3.14};
  Value boolVal{true};
  Value stringVal{"foobar"};
  Value date{std::chrono::milliseconds{1704}};
  Value object{Object{}};
  Value array{Array{1, 2, 3}};

  Value object_document = Object{{"undefined", undefined},
                                 {"integer_32bit", integer_32bit},
                                 {"integer_64bit", integer_64bit},
                                 {"doubleVal", doubleVal},
                                 {"boolVal", boolVal},
                                 {"stringVal", stringVal},
                                 {"date", date},
                                 {"object", object},
                                 {"array", array}};
  std::string str;
  for (int i = 0; i < 100000; i++) {
    str = object_document.toBSON();
    object_document = Value::fromBSON(str);
  }
}

TEST(BSON, size) {
  Value b1{"abc", 3};
  EXPECT_EQ(3ul, b1.size());

  Value b2{"abc\0abc", 7};
  EXPECT_EQ(7ul, b2.size());

  Value b3{"abc"};
  EXPECT_EQ(3ul, b3.size());

  Value b4 = Array{1, 2, 3};
  EXPECT_EQ(3ul, b4.size());

  Value b5 = Object{{"a", "a"}, {"b", "b"}, {"c", "c"}};
  EXPECT_EQ(3ul, b5.size());
}
