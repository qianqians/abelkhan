PROJECT(center)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(server_header_list 
    svrmanager.h
    logicsvrmanager.h
    gm_msg_handle.h
    logic_svr_msg_handle.h
    svr_msg_handle.h
    gmmanager.h)

SET(server_src_list 
    center.cpp
    svrmanager.cpp
    logicsvrmanager.cpp
    gmmanager.cpp)

add_executable(center ${server_src_list} ${server_header_list})
target_link_libraries(center pool)
target_link_libraries(center container)
target_link_libraries(center juggle)