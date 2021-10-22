cmake_minimum_required (VERSION 2.8.11)

project (libbson)

set( CMAKE_COLOR_MAKEFILE ON )
include_directories( ${CMAKE_CURRENT_SOURCE_DIR}/include )

set( SRC ${CMAKE_CURRENT_SOURCE_DIR}/src/sources/Value.cpp
				 ${CMAKE_CURRENT_SOURCE_DIR}/src/sources/bson.cpp
				 ${CMAKE_CURRENT_SOURCE_DIR}/src/sources/json.cpp )

set( CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall --std=c++11 -fPIC -O3" )

add_library( bson_static STATIC ${SRC} )
set_target_properties( bson_static PROPERTIES OUTPUT_NAME bson )

set(CPACK_GENERATOR "DEB")
set(CPACK_SET_DESTDIR ON)
set(CPACK_DEBIAN_PACKAGE_MAINTAINER "Tino Rusch")
set(CPACK_PACKAGE_VERSION "1.0.0")
set(CPACK_PACKAGE_VERSION_MAJOR "1")
set(CPACK_PACKAGE_VERSION_MINOR "0")
set(CPACK_PACKAGE_VERSION_PATCH "0")
include(CPack)
