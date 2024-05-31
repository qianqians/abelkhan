/*
 * hubmanager.cpp
 * qianqians
 * 2024/5/30
 */
#include "hubmanager.h"

namespace dbproxy
{

hubmanager::hubmanager() {
}

std::shared_ptr<hubproxy> hubmanager::reg_hub(std::shared_ptr<abelkhan::Ichannel> ch, std::string name)
{
	auto _hubproxy = std::make_shared<hubproxy>(ch);
	hubproxys_name[name] = _hubproxy;
	hubproxys[ch] = _hubproxy;

	return _hubproxy;
}

void hubmanager::on_hub_closed(std::string name)
{
	if (std::find(closed_hub_list.begin(), closed_hub_list.end(), name) == closed_hub_list.end())
	{
		closed_hub_list.push_back(name);
	}
}

bool hubmanager::get_hub(std::shared_ptr<abelkhan::Ichannel> ch, std::shared_ptr<hubproxy>& _proxy)
{
	auto it = hubproxys.find(ch);
	if (it != hubproxys.end()) {
		_proxy = it->second;
		return true;
	}

	_proxy = nullptr;
	return false;
}

bool hubmanager::all_hub_closed()
{
	return hubproxys_name.size() <= closed_hub_list.size();
}

}