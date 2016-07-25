# Install script for directory: C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/libbson")
endif()
string(REGEX REPLACE "/$" "" CMAKE_INSTALL_PREFIX "${CMAKE_INSTALL_PREFIX}")

# Set the install configuration name.
if(NOT DEFINED CMAKE_INSTALL_CONFIG_NAME)
  if(BUILD_TYPE)
    string(REGEX REPLACE "^[^A-Za-z0-9_]+" ""
           CMAKE_INSTALL_CONFIG_NAME "${BUILD_TYPE}")
  else()
    set(CMAKE_INSTALL_CONFIG_NAME "Release")
  endif()
  message(STATUS "Install configuration: \"${CMAKE_INSTALL_CONFIG_NAME}\"")
endif()

# Set the component getting installed.
if(NOT CMAKE_INSTALL_COMPONENT)
  if(COMPONENT)
    message(STATUS "Install component: \"${COMPONENT}\"")
    set(CMAKE_INSTALL_COMPONENT "${COMPONENT}")
  else()
    set(CMAKE_INSTALL_COMPONENT)
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE PROGRAM FILES
    "C:/Program Files (x86)/Microsoft Visual Studio 14.0/VC/redist/x86/Microsoft.VC140.CRT/msvcp140.dll"
    "C:/Program Files (x86)/Microsoft Visual Studio 14.0/VC/redist/x86/Microsoft.VC140.CRT/vcruntime140.dll"
    )
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  if("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Debug/bson-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Release/bson-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/MinSizeRel/bson-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/RelWithDebInfo/bson-1.0.lib")
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  if("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Debug/libbson-1.0.dll")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Release/libbson-1.0.dll")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/MinSizeRel/libbson-1.0.dll")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/RelWithDebInfo/libbson-1.0.dll")
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  if("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Debug/bson-static-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/Release/bson-static-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/MinSizeRel/bson-static-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/RelWithDebInfo/bson-static-1.0.lib")
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/libbson-1.0" TYPE FILE FILES
    "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/src/bson/bson-config.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/src/bson/bson-stdint.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/src/bson/bson-version.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bcon.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-atomic.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-clock.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-compat.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-context.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-endian.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-error.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-iter.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-json.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-keys.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-macros.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-md5.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-memory.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-oid.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-reader.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-stdint-win32.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-string.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-types.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-utf8.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-value.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-version-functions.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver/src/libbson//src/bson/bson-writer.h"
    )
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/pkgconfig" TYPE FILE FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/libbson-1.0.pc")
endif()

if(CMAKE_INSTALL_COMPONENT)
  set(CMAKE_INSTALL_MANIFEST "install_manifest_${CMAKE_INSTALL_COMPONENT}.txt")
else()
  set(CMAKE_INSTALL_MANIFEST "install_manifest.txt")
endif()

string(REPLACE ";" "\n" CMAKE_INSTALL_MANIFEST_CONTENT
       "${CMAKE_INSTALL_MANIFEST_FILES}")
file(WRITE "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_bson/${CMAKE_INSTALL_MANIFEST}"
     "${CMAKE_INSTALL_MANIFEST_CONTENT}")
