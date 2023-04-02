PROJECT(gate)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

SET(EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)
SET(LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)

set(gc_INCLUDE_PATH ../../cpp_component/3rdparty/bdwgc/include CACHE PATH "gc include path")
INCLUDE_DIRECTORIES(${gc_INCLUDE_PATH})

set(crossguid_INCLUDE_PATH ../../cpp_component/3rdparty/crossguid/include CACHE PATH "crossguid include path")
INCLUDE_DIRECTORIES(${crossguid_INCLUDE_PATH})

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

set(gate_protocol_header_path ../../protocol/cpp CACHE PATH "gate protocol include path")
INCLUDE_DIRECTORIES(${gate_protocol_header_path})

set(gate_header_path ../../cpp_component/component/gate CACHE PATH "gate include path")
INCLUDE_DIRECTORIES(${gate_header_path})

AUX_SOURCE_DIRECTORY(. DIR_SRC)
file(GLOB header_test_files ./ ./*.h)

set(LIBS_DEBUG_PATH ${PROJECT_SOURCE_DIR}//..//..//cpp_component//lib_debug CACHE PATH "lib debug path")
set(LIBS_RELEASE_PATH ${PROJECT_SOURCE_DIR}//..//..//cpp_component//lib CACHE PATH "lib release path")

SET(header_files
    ${header_test_files}
)

SET(SRC_LIST 
    ${DIR_SRC}
)

if (MSVC_VERSION GREATER_EQUAL "1900")
    include(CheckCXXCompilerFlag)
    CHECK_CXX_COMPILER_FLAG("/std:c++latest" _cpp_latest_flag_supported)
    if (_cpp_latest_flag_supported)
        add_compile_options("/std:c++latest")
		add_compile_options(/wd4200)
		add_compile_options(/wd4267)
		add_compile_options(/wd4996)
    endif()
endif()

ADD_EXECUTABLE(gate ${SRC_LIST} ${header_files})
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/libssl64MDd optimized ${LIBS_RELEASE_PATH}/libssl64MD)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/libcrypto64MDd optimized ${LIBS_RELEASE_PATH}/libcrypto64MD)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/hiredis optimized ${LIBS_RELEASE_PATH}/hiredis)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/hiredis_cluster optimized ${LIBS_RELEASE_PATH}/hiredis_cluster)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/crossguid-dgb optimized ${LIBS_RELEASE_PATH}/crossguid)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/spdlogd optimized ${LIBS_RELEASE_PATH}/spdlog)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/enet optimized ${LIBS_RELEASE_PATH}/enet)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/msgpack11 optimized ${LIBS_RELEASE_PATH}/msgpack11)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/json11 optimized ${LIBS_RELEASE_PATH}/json11)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/cares optimized ${LIBS_RELEASE_PATH}/cares)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/bson optimized ${LIBS_RELEASE_PATH}/bson)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/gate optimized ${LIBS_RELEASE_PATH}/gate)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/gc optimized ${LIBS_RELEASE_PATH}/gc)
target_link_libraries(gate Crypt32 )
target_link_libraries(gate ws2_32)
target_link_libraries(gate winmm)