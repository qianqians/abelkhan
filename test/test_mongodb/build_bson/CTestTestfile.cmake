# CMake generated Testfile for 
# Source directory: C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson
# Build directory: C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson
# 
# This file includes the relevant testing commands required for 
# testing this directory and lists subdirectories to be tested as well.
if("${CTEST_CONFIGURATION_TYPE}" MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
  add_test(test-libbson "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Debug/test-libbson.exe")
elseif("${CTEST_CONFIGURATION_TYPE}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
  add_test(test-libbson "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Release/test-libbson.exe")
elseif("${CTEST_CONFIGURATION_TYPE}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
  add_test(test-libbson "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/MinSizeRel/test-libbson.exe")
elseif("${CTEST_CONFIGURATION_TYPE}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
  add_test(test-libbson "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/RelWithDebInfo/test-libbson.exe")
else()
  add_test(test-libbson NOT_AVAILABLE)
endif()
