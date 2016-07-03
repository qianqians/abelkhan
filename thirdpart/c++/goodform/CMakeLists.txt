cmake_minimum_required(VERSION 2.8.4)
project(goodform)

include_directories("./include")

set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_HOME_DIRECTORY}/lib)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

#set(CMAKE_CXX_COMPILER "/opt/local/bin/g++-mp-4.9")
#set(CMAKE_CC_COMPILER "/opt/local/bin/gcc-mp-4.9")

set(HEADER_FILES
  include/form.hpp
  include/variant.hpp
  include/msgpack.hpp
  include/json.hpp)

set(SOURCE_FILES
  src/variant.cpp
  src/msgpack.cpp
  src/json.cpp)

add_library(goodform SHARED ${SOURCE_FILES} ${HEADER_FILES})

add_library(goodform-static STATIC ${SOURCE_FILES} ${HEADER_FILES})

add_executable(variant-test tests/variant-test.cpp ${SOURCE_FILES} ${HEADER_FILES})