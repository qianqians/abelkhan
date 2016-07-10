PROJECT(hub)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(server_header_list 
    centerproxy.h
    logicsvrmanager.h
    logic_svr_msg_handle.h)

SET(server_src_list 
    hub.cpp
    centerproxy.cpp
    logicsvrmanager.cpp)

add_executable(hub ${server_src_list} ${server_header_list})
target_link_libraries(hub pool)
target_link_libraries(hub container)