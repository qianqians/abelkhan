PROJECT(juggle)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(BOOST_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//..//thirdpart//c++//boost CACHE PATH "boost include path")
INCLUDE_DIRECTORIES(${BOOST_INCLUDE_PATH})

set(BOOST_LIBS_PATH ${PROJECT_SOURCE_DIR}//..//..//thirdpart//c++//boost//stage//lib CACHE PATH "boost lib path")
link_directories(${BOOST_LIBS_PATH})

SET(EXECUTABLE_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)
SET(LIBRARY_OUTPUT_PATH ${PROJECT_BINARY_DIR}/bin)

AUX_SOURCE_DIRECTORY(. DIR_SRC)

SET(header_files
    Icaller.h
    Ichannel.h
    Imodule.h
    process_.h
)

SET(SRC_LIST 
    ${DIR_SRC}
)

ADD_LIBRARY(juggle ${SRC_LIST} ${header_files})