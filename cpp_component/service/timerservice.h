
#ifndef _timerservice_h
#define _timerservice_h

#include <functional>
#include <map>
#include <vector>
#include <mutex>

#include <ringque.h>

#include "msec_time.h"

namespace service
{

class timerservice {
public:
	class timerimpl {
	public:
		bool is_del = false;
		std::function< void(int64_t) > cb;
	};

private:
	std::map<int64_t, std::shared_ptr<timerimpl> > cbs;

	concurrent::ringque<std::pair<int64_t, std::shared_ptr<timerimpl> > > add_cbs;
	std::vector<int64_t> remove;

public:
	timerservice() 
	{
		Tick = msec_time();
	}

	std::shared_ptr<timerimpl> addticktimer(int64_t _tick, std::function< void(int64_t) > cb)
	{
		_tick += Tick;
		auto timpl = std::make_shared<timerimpl>();
		timpl->cb = cb;
		add_cbs.push(std::make_pair(_tick, timpl));
		return timpl;
	}

	int64_t poll()
	{
		Tick = msec_time();

		while (true)
		{
			std::pair<int64_t, std::shared_ptr<timerimpl> > cb;
			if (!add_cbs.pop(cb)) {
				break;
			}

			while (cbs.find(cb.first) != cbs.end())
			{
				cb.first++;
			}

			cbs.insert(cb);
		}

		for (auto it = cbs.begin(); it != cbs.end(); it++)
		{
			if (it->first <= Tick)
			{
				if (!it->second->is_del) {
					it->second->cb(it->first);
				}
				remove.push_back(it->first);
			}
			else {
				break;
			}
		}

		for (auto key : remove)
		{
			cbs.erase(key);
		}
		remove.clear();

		return Tick;
	}

public:
	time_t Tick;

};

}

#endif
