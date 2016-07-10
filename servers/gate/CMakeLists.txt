PROJECT(gate)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(server_header_list 
    centerproxy.h
    client_msg_handle.h
    clientmanager.h
    logicsvrmanager.h
    logic_svr_msg_handle.h)

SET(server_src_list 
    gate.cpp
    centerproxy.cpp
    clientmanager.cpp
    logicsvrmanager.cpp)

add_executable(gate ${server_src_list} ${server_header_list})
target_link_libraries(gate pool)
target_link_libraries(gate container)