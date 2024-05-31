/*
 * hubmanager.h
 * qianqians
 * 2024/5/30
 */
#ifndef _hubmanager_h_
#define _hubmanager_h_

#include <unordered_map>

#include "hubproxy.h"

namespace dbproxy
{

class hubmanager
{
private:
	std::unordered_map<std::string, std::shared_ptr<hubproxy> > hubproxys_name;
	std::unordered_map<std::shared_ptr<abelkhan::Ichannel>, std::shared_ptr<hubproxy> > hubproxys;
		
	std::vector<std::string> closed_hub_list;

public:
	hubmanager();

	std::shared_ptr<hubproxy> reg_hub(std::shared_ptr<abelkhan::Ichannel> ch, std::string name);

	void on_hub_closed(std::string name);

	bool get_hub(std::shared_ptr<abelkhan::Ichannel> ch, std::shared_ptr<hubproxy>& _proxy);

	bool all_hub_closed();
};

}

#endif // !_hubmanager_h_