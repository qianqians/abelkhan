
#ifndef _enetchannel_h
#define _enetchannel_h

#include <list>
#include <iostream>
#include <any>

#include <spdlog/spdlog.h>
#include <enet/enet.h>

#include <angmalloc.h>

#include "JsonParse.h"
#include "Ichannel.h"

namespace service
{

class enetchannel : public juggle::Ichannel {
public:
	enetchannel(ENetHost * host, ENetPeer * peer)
	{
		_host = host;
		_peer = peer;
	}

	~enetchannel(){
	}

public:
	void disconnect()
	{
	}

	void recv(char * data, size_t length)
	{
		uint8_t* tmp = (uint8_t*)data;
		uint32_t len = (uint32_t)tmp[0] | ((uint32_t)tmp[1] << 8) | ((uint32_t)tmp[2] << 16) | ((uint32_t)tmp[3] << 24);

		if ((len + 4) <= length)
		{
			auto json_buff = &data[4];
			std::string json_str((char*)(json_buff), len);
			try
			{
				spdlog::trace("recv:{0}", json_str);
				Fossilizid::JsonParse::JsonObject obj;
				Fossilizid::JsonParse::unpacker(obj, json_str);
				que.push_back(std::any_cast<Fossilizid::JsonParse::JsonArray>(obj));
			}
			catch (Fossilizid::JsonParse::jsonformatexception e)
			{
				spdlog::error("enetchannel recv error:{0}", json_str);
				disconnect();

				return;
			}
		}
	}

	bool pop(Fossilizid::JsonParse::JsonArray  & out)
	{
		if (que.empty())
		{
			return false;
		}

		out = que.front();
		que.pop_front();

		return true;
	}

	void push(Fossilizid::JsonParse::JsonArray in)
	{
		try {
			std::string data;
			Fossilizid::JsonParse::pack(in, data);
			size_t len = data.size();

			unsigned char * _data = (unsigned char*)angmalloc(len + 4);
			_data[0] = len & 0xff;
			_data[1] = len >> 8 & 0xff;
			_data[2] = len >> 16 & 0xff;
			_data[3] = len >> 24 & 0xff;
			memcpy(&_data[4], data.c_str(), data.size());
			size_t datasize = len + 4;

			ENetPacket* packet = enet_packet_create(_data, datasize, ENET_PACKET_FLAG_RELIABLE);
			enet_peer_send(_peer, 0, packet);
			enet_host_flush(_host);

			spdlog::trace("push:{0}", data);

			angfree(_data);
		}
		catch (std::exception e) {
			spdlog::error("enetchannel push exception error:{0}", e.what());
		}
	}

private:
	ENetHost * _host;
	ENetPeer * _peer;

	std::list<Fossilizid::JsonParse::JsonArray> que;

};

}

#endif
