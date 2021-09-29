/*
 * gc_poll.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _gc_poll_h
#define _gc_poll_h

#include <functional>

namespace service {

	void gc_put(std::function<void()> gc_fn);
	void gc_poll();

}

#endif //_clientmanager_h
