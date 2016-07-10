PROJECT(dbproxy)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(server_header_list 
    centerproxy.h
    mongodb_proxy.h
    logicsvrmanager.h
    logic_svr_msg_handle.h)

SET(server_src_list 
    centerproxy.cpp
    dbproxy.cpp
    mongodb_proxy.cpp
    logicsvrmanager.cpp)

add_executable(dbproxy ${server_src_list} ${server_header_list})
target_link_libraries(dbproxy pool)
target_link_libraries(dbproxy container)