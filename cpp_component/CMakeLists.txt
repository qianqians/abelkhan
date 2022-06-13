PROJECT(abelkhan_cpp)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

if (MSVC_VERSION GREATER_EQUAL "1900")
    include(CheckCXXCompilerFlag)
    CHECK_CXX_COMPILER_FLAG("/std:c++latest" _cpp_latest_flag_supported)
    if (_cpp_latest_flag_supported)
        add_compile_options("/std:c++latest")
    endif()
endif()

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /bigobj")

set(BOOST_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//boost CACHE PATH "boost include path")
INCLUDE_DIRECTORIES(${BOOST_INCLUDE_PATH})

set(websocketpp_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//websocketpp CACHE PATH "websocketpp include path")
INCLUDE_DIRECTORIES(${websocketpp_INCLUDE_PATH})

set(enet_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//enet//include CACHE PATH "enet include path")
INCLUDE_DIRECTORIES(${enet_INCLUDE_PATH})

set(spdlog_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//spdlog//include CACHE PATH "spdlog include path")
INCLUDE_DIRECTORIES(${spdlog_INCLUDE_PATH})

set(openssl_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//openssl//include CACHE PATH "openssl include path")
INCLUDE_DIRECTORIES(${openssl_INCLUDE_PATH})

set(json11_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//json11 CACHE PATH "json11 include path")
INCLUDE_DIRECTORIES(${json11_INCLUDE_PATH})

set(libbson_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//libbson//include CACHE PATH "libbson include path")
INCLUDE_DIRECTORIES(${libbson_INCLUDE_PATH})

set(c_ares_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//c-ares//include CACHE PATH "c-ares include path")
INCLUDE_DIRECTORIES(${c_ares_INCLUDE_PATH})

set(hiredis_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//3rdparty//hiredis-cluster CACHE PATH "hiredis include path")
INCLUDE_DIRECTORIES(${hiredis_INCLUDE_PATH})

set(msgpack11_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//tools//juggle//cpp//3rdparty//msgpack11 CACHE PATH "msgpack11 include path")
INCLUDE_DIRECTORIES(${msgpack11_INCLUDE_PATH})

set(concurrent_INCLUDE_PATH ${PROJECT_SOURCE_DIR}//..//tools//juggle//cpp//concurrent CACHE PATH "concurrent include path")
INCLUDE_DIRECTORIES(${concurrent_INCLUDE_PATH})

add_subdirectory(./component/hub)
add_subdirectory(./component/gate)