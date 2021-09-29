
#ifndef _juggleservice_h
#define _juggleservice_h

#include "process_.h"

#include <memory>
#include <vector>

namespace service
{

class juggleservice {
public:
	void add_process(std::shared_ptr<juggle::process> pc)
	{
		_ps.push_back(pc);
	}

	void poll()
	{
		for (auto pc : _ps)
		{
			pc->poll();
		}
	}

private:
	std::vector< std::shared_ptr<juggle::process> > _ps;

};

}

#endif
