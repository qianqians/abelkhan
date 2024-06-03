/*
 * hubproxy.cpp
 * qianqians
 * 2024/5/30
 */
#include "hubproxy.h"

namespace dbproxy
{
hubproxy::hubproxy(std::shared_ptr<abelkhan::Ichannel> ch) : _caller(ch, service::_modulemng)
{
	_ch = ch;
}

void hubproxy::ack_get_object_info(std::string callbackid, bson_t* object_info)
{
	auto bin = bson_get_data(object_info);
	std::vector<uint8_t> bin1;
	bin1.resize(object_info->len);
	memcpy(bin1.data(), bin, object_info->len);

	_caller.ack_get_object_info(callbackid, bin1);

	bson_destroy(object_info);
}

void hubproxy::ack_get_object_info_end(std::string callbackid)
{
	_caller.ack_get_object_info_end(callbackid);
}
}

