/*
 * hubproxy.h
 * qianqians
 * 2024/5/30
 */
#ifndef _hubproxy_h_
#define _hubproxy_h_

#include <bson/bson.h>

#include <abelkhan.h>
#include <modulemng_handle.h>

#include <dbproxy.h>

namespace dbproxy
{

class hubproxy
{
private:
	std::shared_ptr<abelkhan::Ichannel> _ch;
	abelkhan::dbproxy_call_hub_caller _caller;
	
public:
	hubproxy(std::shared_ptr<abelkhan::Ichannel> ch);

	void ack_get_object_info(std::string callbackid, bson_t* object_info);

	void ack_get_object_info_end(std::string callbackid);

};

}

#endif // !_hubproxy_h_