/*
 * qianqians
 * 2016/7/5
 * juggleservice.h
 */
#ifndef _juggleservice_h
#define _juggleservice_h

#include <list>
#include <boost/shared_ptr.hpp>

#include <process_.h>

#include "service.h"

namespace service {

class juggleservice : public service {
public:
	juggleservice() {
	}

	~juggleservice(){
	}

	void add_process(boost::shared_ptr<juggle::process> _process){
		process_set.push_back(_process);
	}

	void poll(int64_t tick){
		for(auto p : process_set){
			p->poll();
		}
	}

private:
	std::list<boost::shared_ptr<juggle::process> > process_set;

};

}

#endif // !_juggleservice_h
