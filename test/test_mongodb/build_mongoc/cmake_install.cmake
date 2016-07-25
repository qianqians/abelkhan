# Install script for directory: C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/libmongoc")
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
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/Debug/mongoc-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/Release/mongoc-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/MinSizeRel/mongoc-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/RelWithDebInfo/mongoc-1.0.lib")
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  if("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/Debug/libmongoc-1.0.dll")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/Release/libmongoc-1.0.dll")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/MinSizeRel/libmongoc-1.0.dll")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE SHARED_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/RelWithDebInfo/libmongoc-1.0.dll")
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  if("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/Debug/mongoc-static-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/Release/mongoc-static-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/MinSizeRel/mongoc-static-1.0.lib")
  elseif("${CMAKE_INSTALL_CONFIG_NAME}" MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
    file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/RelWithDebInfo/mongoc-static-1.0.lib")
  endif()
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/libmongoc-1.0" TYPE FILE FILES
    "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/src/mongoc/mongoc-config.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/src/mongoc/mongoc-version.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-apm.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-apm-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-bulk-operation.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-client.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-client-pool.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-collection.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-cursor.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-database.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-error.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-flags.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-find-and-modify.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-gridfs.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-gridfs-file.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-gridfs-file-page.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-gridfs-file-list.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-host-list.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-init.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-index.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-iovec.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-log.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-matcher.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-opcode.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-opcode-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-read-concern.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-read-prefs.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-server-description.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-socket.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-socket-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-tls-openssl-bio-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-tls-openssl-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-tls-openssl.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-tls-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-buffered.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-file.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-gridfs.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-stream-socket.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-trace.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-trace-private.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-uri.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-version-functions.h"
    "C:/Users/abelk/Documents/workspace/abelkhan/thirdpart/c++/mongo-c-driver//src/mongoc/mongoc-write-concern.h"
    )
endif()

if("${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/pkgconfig" TYPE FILE FILES "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/libmongoc-1.0.pc")
endif()

if(CMAKE_INSTALL_COMPONENT)
  set(CMAKE_INSTALL_MANIFEST "install_manifest_${CMAKE_INSTALL_COMPONENT}.txt")
else()
  set(CMAKE_INSTALL_MANIFEST "install_manifest.txt")
endif()

string(REPLACE ";" "\n" CMAKE_INSTALL_MANIFEST_CONTENT
       "${CMAKE_INSTALL_MANIFEST_FILES}")
file(WRITE "C:/Users/abelk/Documents/workspace/abelkhan/test/test_mongodb/build_mongoc/${CMAKE_INSTALL_MANIFEST}"
     "${CMAKE_INSTALL_MANIFEST_CONTENT}")
