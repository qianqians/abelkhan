PROJECT(gate)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

SET(EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)
SET(LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)

set(protocol_header_path ..//..//..//protocol//cpp CACHE PATH "protocol include path")
INCLUDE_DIRECTORIES(${protocol_header_path})
file(GLOB protcol_header_files ../../../protocol/cpp/ ../../../protocol/cpp/*.h)
AUX_SOURCE_DIRECTORY(../../../protocol/cpp/ protocol_DIR_SRC)

set(juggle_header_path ..//..//..//tools//juggle//cpp CACHE PATH "juggle include path")
INCLUDE_DIRECTORIES(${juggle_header_path})
file(GLOB juggle_header_files ../../../tools/juggle/cpp/ ../../../tools/juggle/cpp/*.h)
AUX_SOURCE_DIRECTORY(../../../tools/juggle/cpp/ juggle_DIR_SRC)

set(time_header_path ..//..//time CACHE PATH "time include path")
INCLUDE_DIRECTORIES(${time_header_path})
file(GLOB time_header_files ../../time/ ../../time/*.h)
AUX_SOURCE_DIRECTORY(../../time/ time_DIR_SRC)

set(config_header_path ..//..//config CACHE PATH "config include path")
INCLUDE_DIRECTORIES(${config_header_path})
file(GLOB config_header_files ../../config/ ../../config/*.h)

set(service_header_path ..//..//service CACHE PATH "service include path")
INCLUDE_DIRECTORIES(${service_header_path})
file(GLOB service_header_files ../../service/ ../../service/*.h)
AUX_SOURCE_DIRECTORY(../../service service_DIR_SRC)

set(module_header_path ..//..//module CACHE PATH "module include path")
INCLUDE_DIRECTORIES(${module_header_path})
file(GLOB module_header_files ../../module/ ../../module/*.h)

set(log_header_path ..//..//log CACHE PATH "log include path")
INCLUDE_DIRECTORIES(${log_header_path})
file(GLOB log_header_files ../../log/ ../../log/*.h)

AUX_SOURCE_DIRECTORY(. DIR_SRC)
file(GLOB header_gate_files ./ ./*.h)

set(LIBS_DEBUG_PATH ${PROJECT_SOURCE_DIR}//..//..//lib_debug CACHE PATH "lib debug path")
set(LIBS_RELEASE_PATH ${PROJECT_SOURCE_DIR}//..//..//lib CACHE PATH "lib release path")

SET(header_files
    ${header_gate_files}
    ${module_header_files}
    ${service_header_files}
    ${juggle_header_files}
    ${protocol_header_files}
    ${config_header_files}
    ${time_header_files}
    ${log_header_files}
)

SET(SRC_LIST 
    ${DIR_SRC}
    ${juggle_DIR_SRC}
    ${service_DIR_SRC}
    ${protocol_DIR_SRC}
    ${time_DIR_SRC}
)

ADD_EXECUTABLE(gate ${SRC_LIST} ${header_files})
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/libssl64MDd optimized ${LIBS_RELEASE_PATH}/libssl64MD)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/libcrypto64MDd optimized ${LIBS_RELEASE_PATH}/libcrypto64MD)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/spdlogd optimized ${LIBS_RELEASE_PATH}/spdlog)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/enet optimized ${LIBS_RELEASE_PATH}/enet)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/msgpack11 optimized ${LIBS_RELEASE_PATH}/msgpack11)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/json11 optimized ${LIBS_RELEASE_PATH}/json11)
target_link_libraries(gate debug ${LIBS_DEBUG_PATH}/cares optimized ${LIBS_RELEASE_PATH}/cares)
target_link_libraries(gate Crypt32 )
target_link_libraries(gate ws2_32)
target_link_libraries(gate winmm)