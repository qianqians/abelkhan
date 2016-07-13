/*
 * qianqians
 * 2016/7/4
 * timerservice.h
 */
#ifndef _timerservice_h
#define _timerservice_h

#include <time.h>

#include <vector>
#include <map>
#include <functional>

#include "service.h"

namespace service {

class timerservice : public service{
public:
	timerservice(int64_t tick) {
		ticktime = tick;
	}

	~timerservice(){
	}

	void addticktime(int64_t process, std::function<void(int64_t)> handle){
		tickHandledict.insert(std::make_pair(process, handle));
	}

	void adddatetime(tm process, std::function<void(tm)> handle){
		timeHandledict.insert(std::make_pair(mktime(&process), handle));
	}


	void poll(int64_t tick) {
		ticktime = tick;

		{
			std::vector<int64_t> v;

			auto itend = tickHandledict.upper_bound(tick);
			for (auto it = tickHandledict.begin(); it != itend; it++) {
				it->second(tick);
				v.push_back(it->first);
			}

			for (auto itick : v) {
				tickHandledict.erase(itick);
			}
		}

		{
			tm t;
			time_t ttime = time(0);
			localtime_s(&t, &ttime);

			std::vector<time_t> v;

			auto itend = timeHandledict.upper_bound(ttime);
			for (auto it = timeHandledict.begin(); it != itend; it++) {
				it->second(t);
				v.push_back(it->first);
			}

			for (auto itick : v) {
				timeHandledict.erase(itick);
			}
		}
	}

public:
	int64_t ticktime;

private:
	std::map<int64_t, std::function<void(int64_t)> > tickHandledict;
	std::map<time_t, std::function<void(tm)> > timeHandledict;

};

}

#endif // !_timerservice_h