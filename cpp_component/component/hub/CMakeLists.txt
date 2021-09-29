PROJECT(hub)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

SET(EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)
SET(LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)

set(protcol_header_path ..//..//..//protcol//caller CACHE PATH "protcol include path")
INCLUDE_DIRECTORIES(${protcol_header_path})
file(GLOB protcol_header_files ../../../protcol/caller/ ../../../protcol/caller/*.h)

set(juggle_header_path ..//..//..//tools//juggle//cpp CACHE PATH "juggle include path")
INCLUDE_DIRECTORIES(${juggle_header_path})
file(GLOB juggle_header_files ../../../tools/juggle/cpp/ ../../../tools/juggle/cpp/*.h)
AUX_SOURCE_DIRECTORY(../../../tools/juggle/cpp/ juggle_DIR_SRC)

set(time_header_path ..//..//time CACHE PATH "time include path")
INCLUDE_DIRECTORIES(${time_header_path})
file(GLOB time_header_files ../../time/ ../../time/*.h)

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
file(GLOB header_client_files ./ ./*.h)

SET(header_files
    ${header_client_files}
    ${module_header_files}
    ${service_header_files}
    ${juggle_header_files}
    ${protcol_header_files}
    ${config_header_files}
    ${time_header_files}
    ${log_header_files}
)

SET(SRC_LIST 
    ${DIR_SRC}
    ${juggle_DIR_SRC}
    ${service_DIR_SRC}
)

ADD_LIBRARY(hub ${SRC_LIST} ${header_files})