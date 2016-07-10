PROJECT(pool)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

AUX_SOURCE_DIRECTORY(. DIR_SRC)

SET(header_files 
    mempool.h
    objpool.h
    factory.h
)

SET(SRC_LIST 
    ${DIR_SRC}
)

ADD_LIBRARY(pool ${SRC_LIST} ${header_files})