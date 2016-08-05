PROJECT(servers)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

ADD_DEFINITIONS(-D_SCL_SECURE_NO_WARNINGS)
ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS)
ADD_DEFINITIONS(-D___mempool___)

set(tools_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//common//tools//c++ CACHE PATH "tools include path")
INCLUDE_DIRECTORIES(${tools_INCLUDE_PATH})

set(module_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//common//juggle_protcol//c++ CACHE PATH "module include path")
INCLUDE_DIRECTORIES(${module_INCLUDE_PATH})

set(lock_free_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//fossilizid CACHE PATH "lock_free include path")
INCLUDE_DIRECTORIES(${lock_free_INCLUDE_PATH})

set(lock_free_LIBS_PATH ${PROJECT_SOURCE_DIR}//..//test//test_fossilizid//build//bin CACHE PATH "lock-free lib path")
link_directories(${lock_free_LIBS_PATH})

set(jsonparse_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//jsonparse//cpp CACHE PATH "jsonparse include path")
INCLUDE_DIRECTORIES(${jsonparse_INCLUDE_PATH})

set(juggle_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//juggle//c++ CACHE PATH "juggle include path")
INCLUDE_DIRECTORIES(${juggle_INCLUDE_PATH})

set(juggle_LIBS_PATH ${PROJECT_SOURCE_DIR}//..//test//test_juggle//build//bin CACHE PATH "juggle lib path")
link_directories(${juggle_LIBS_PATH})

set(service_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//common//service//c++ CACHE PATH "service include path")
INCLUDE_DIRECTORIES(${service_INCLUDE_PATH})

set(BOOST_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//thirdpart//c++//boost CACHE PATH "boost include path")
INCLUDE_DIRECTORIES(${BOOST_INCLUDE_PATH})

set(BOOST_LIBS_PATH ${PROJECT_SOURCE_DIR}//..//thirdpart//c++//boost//stage//lib CACHE PATH "boost lib path")
link_directories(${BOOST_LIBS_PATH})

SET(EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)
SET(LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)

ADD_SUBDIRECTORY(center)
ADD_SUBDIRECTORY(dbproxy)
ADD_SUBDIRECTORY(gate)