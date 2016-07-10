PROJECT(routing)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(server_header_list 
    centerproxy.h
    logicsvrmanager.h
    logic_svr_msg_handle.h)

SET(server_src_list 
    routing.cpp
    centerproxy.cpp
    logicsvrmanager.cpp)

add_executable(routing ${server_src_list} ${server_header_list})
target_link_libraries(routing pool)
target_link_libraries(routing container)