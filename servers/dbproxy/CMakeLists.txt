PROJECT(dbproxy)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(bson_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//..//test//test_mongodb//libbson//include//libbson-1.0 CACHE PATH "bson include path")
INCLUDE_DIRECTORIES(${bson_INCLUDE_PATH})

set(bson_LIBS_PATH ${PROJECT_SOURCE_DIR}//..//..//test//test_mongodb//libbson//lib CACHE PATH "bson lib path")
link_directories(${bson_LIBS_PATH})

set(mongoc_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//..//test//test_mongodb//libmongoc//include//libmongoc-1.0 CACHE PATH "mongoc include path")
INCLUDE_DIRECTORIES(${mongoc_INCLUDE_PATH})

set(mongoc_LIBS_PATH ${PROJECT_SOURCE_DIR}//..//..//test//test_mongodb//libmongoc//lib CACHE PATH "mongoc lib path")
link_directories(${mongoc_LIBS_PATH})

set(server_header_list 
    centerproxy.h
    mongodb_proxy.h
    logicsvrmanager.h
    center_msg_handle.h
    logic_svr_msg_handle.h
    closehandle.h
    hub_svr_msg_handle.h
    hubsvrmanager.h)

SET(server_src_list 
    centerproxy.cpp
    dbproxy.cpp
    mongodb_proxy.cpp
    logicsvrmanager.cpp
    closehandle.cpp
    hubsvrmanager.cpp)

add_executable(dbproxy ${server_src_list} ${server_header_list})
target_link_libraries(dbproxy pool)
target_link_libraries(dbproxy container)
target_link_libraries(dbproxy juggle)
target_link_libraries(dbproxy bson-static-1.0)
target_link_libraries(dbproxy mongoc-static-1.0)