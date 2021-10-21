/*
 * qianqians
 * 2016-7-5
 * dbproxyproxy.cpp
 */

#include "dbproxyproxy.h"

namespace hub {
	
std::unordered_map<std::string, std::function<void(BSON::Array)> > dbproxyproxy::get_object_info_callback;
std::unordered_map<std::string, std::function<void()> > dbproxyproxy::get_object_info_end_callback;

}