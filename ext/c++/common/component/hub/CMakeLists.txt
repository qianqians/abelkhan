PROJECT(hub)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

set(server_header_list 
    center_msg_handle.h
    centerproxy.h
    closehandle.h
    dbproxy_msg_handle.h
    dbproxyproxy.h
    gate_msg_handle.h
    gatemanager.h
    gateproxy.h
    logic_svr_msg_handle.h
    logicmanager.h
    logicproxy.h
)

SET(server_src_list 
    centerproxy.cpp
    closehandle.cpp
    dbproxyproxy.cpp
    gatemanager.cpp
    gateproxy.cpp
    hub.cpp
    logicmanager.cpp
    logicproxy.cpp
)

add_executable(hub ${server_src_list} ${server_header_list})
target_link_libraries(hub pool)
target_link_libraries(hub container)
target_link_libraries(hub juggle)