/*
 * qianqians
 * 2016/7/4
 * process.h
 */
#ifndef _process_h
#define _process_h

#include <unordered_map>

#include "Imodule.h"

namespace juggle {

class process {
	process() {

	}



	std::unordered_map<std::string, Imodule> module_set;
};

}

#endif // !_process_h