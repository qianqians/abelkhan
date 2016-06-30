PROJECT(container)
CMAKE_MINIMUM_REQUIRED(VERSION 2.6) 

SET(header_files
    test.h)

SET(test_msque_SRC_LIST 
    test_msque.cpp)

add_executable(test_msque ${test_msque_SRC_LIST} ${header_files})
target_link_libraries(test_msque pool)

SET(test_optimisticque_SRC_LIST 
    test_optimisticque.cpp)

add_executable(test_optimisticque ${test_optimisticque_SRC_LIST} ${header_files})
target_link_libraries(test_optimisticque pool)
target_link_libraries(test_optimisticque container)

SET(test_ringque_SRC_LIST 
    test_ringque.cpp)

add_executable(test_ringque ${test_ringque_SRC_LIST} ${header_files})
target_link_libraries(test_ringque pool)
target_link_libraries(test_optimisticque container)

SET(test_small_hash_map_SRC_LIST 
    test_small_hash_map.cpp)

add_executable(test_small_hash_map ${test_small_hash_map_SRC_LIST} ${header_files})
target_link_libraries(test_small_hash_map pool)
target_link_libraries(test_optimisticque container)

SET(test_swapque_SRC_LIST 
    test_swapque.cpp)

add_executable(test_swapque ${test_swapque_SRC_LIST} ${header_files})
target_link_libraries(test_swapque pool)
target_link_libraries(test_optimisticque container)