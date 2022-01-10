PROJECT(test)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

SET(EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)
SET(LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)

set(BOOST_INCLUDE_PATH ../../cpp_component/3rdparty/boost CACHE PATH "boost include path")
INCLUDE_DIRECTORIES(${BOOST_INCLUDE_PATH})

set(spdlog_INCLUDE_PATH ../../cpp_component/3rdparty/spdlog/include CACHE PATH "spdlog include path")
INCLUDE_DIRECTORIES(${spdlog_INCLUDE_PATH})

set(json11_INCLUDE_PATH ../../cpp_component/3rdparty/json11 CACHE PATH "json11 include path")
INCLUDE_DIRECTORIES(${json11_INCLUDE_PATH})

set(libbson_INCLUDE_PATH ../../cpp_component/3rdparty/libbson/include CACHE PATH "libbson include path")
INCLUDE_DIRECTORIES(${libbson_INCLUDE_PATH})

set(msgpack11_INCLUDE_PATH ../../tools/juggle/cpp/3rdparty/msgpack11 CACHE PATH "msgpack11 include path")
INCLUDE_DIRECTORIES(${msgpack11_INCLUDE_PATH})

set(concurrent_INCLUDE_PATH ../../tools/juggle/cpp/concurrent CACHE PATH "concurrent include path")
INCLUDE_DIRECTORIES(${concurrent_INCLUDE_PATH})

set(juggle_header_path ../../tools/juggle/cpp CACHE PATH "juggle include path")
INCLUDE_DIRECTORIES(${juggle_header_path})

set(proto_header_path ../proto/cpp CACHE PATH "proto include path")
INCLUDE_DIRECTORIES(${proto_header_path})
file(GLOB proto_header_files ../proto/cpp/ ../proto/cpp/*.h)
AUX_SOURCE_DIRECTORY(../proto/cpp/ proto_DIR_SRC)

set(time_header_path ../../cpp_component/time CACHE PATH "time include path")
INCLUDE_DIRECTORIES(${time_header_path})

set(config_header_path ../../cpp_component/config CACHE PATH "config include path")
INCLUDE_DIRECTORIES(${config_header_path})

set(service_header_path ../../cpp_component/service CACHE PATH "service include path")
INCLUDE_DIRECTORIES(${service_header_path})

set(module_header_path ../../cpp_component/module CACHE PATH "module include path")
INCLUDE_DIRECTORIES(${module_header_path})

set(log_header_path ../../cpp_component/log CACHE PATH "log include path")
INCLUDE_DIRECTORIES(${log_header_path})

set(hub_protocol_header_path ../../protocol/cpp CACHE PATH "hub protocol include path")
INCLUDE_DIRECTORIES(${hub_protocol_header_path})

set(hub_header_path ../../cpp_component/component/hub CACHE PATH "hub include path")
INCLUDE_DIRECTORIES(${hub_header_path})

AUX_SOURCE_DIRECTORY(. DIR_SRC)
file(GLOB header_test_files ./ ./*.h)

set(LIBS_DEBUG_PATH ${PROJECT_SOURCE_DIR}//..//..//cpp_component//lib_debug CACHE PATH "lib debug path")
set(LIBS_RELEASE_PATH ${PROJECT_SOURCE_DIR}//..//..//cpp_component//lib CACHE PATH "lib release path")

SET(header_files
    ${header_test_files}
    ${proto_header_files}
)

SET(SRC_LIST 
    ${DIR_SRC}
    ${proto_DIR_SRC}
)

ADD_EXECUTABLE(test ${SRC_LIST} ${header_files})
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/libssl64MDd optimized ${LIBS_RELEASE_PATH}/libssl64MD)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/libcrypto64MDd optimized ${LIBS_RELEASE_PATH}/libcrypto64MD)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/spdlogd optimized ${LIBS_RELEASE_PATH}/spdlog)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/enet optimized ${LIBS_RELEASE_PATH}/enet)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/msgpack11 optimized ${LIBS_RELEASE_PATH}/msgpack11)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/json11 optimized ${LIBS_RELEASE_PATH}/json11)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/cares optimized ${LIBS_RELEASE_PATH}/cares)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/bson optimized ${LIBS_RELEASE_PATH}/bson)
target_link_libraries(test debug ${LIBS_DEBUG_PATH}/hub optimized ${LIBS_RELEASE_PATH}/hub)
target_link_libraries(test Crypt32 )
target_link_libraries(test ws2_32)
target_link_libraries(test winmm)