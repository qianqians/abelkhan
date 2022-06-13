/*
 * qianqians
 * 2016-7-12
 * gc_poll.cpp
 */
#include "gc_poll.h"
#include "ringque.h"

namespace service {

static concurrent::ringque<std::function<void()> > gc_fn_que;

void gc_put(std::function<void()> gc_fn)
{
	gc_fn_que.push(gc_fn);
}

void gc_poll()
{
	std::function<void()> gc_fn;
	while (gc_fn_que.pop(gc_fn))
	{
		gc_fn();
	}
}

}