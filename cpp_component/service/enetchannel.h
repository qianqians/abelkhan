#ifndef _enetchannel_h
#define _enetchannel_h

#include <list>
#include <iostream>
#include <any>

#include <spdlog/spdlog.h>
#include <enet/enet.h>

#include <abelkhan.h>

#include "channel_encrypt_decrypt_ondata.h"

namespace service
{

class enetchannel : public abelkhan::Ichannel, public std::enable_shared_from_this<enetchannel> {
public:
	enetchannel(ENetHost * host, ENetPeer * peer)
	{
		_host = host;
		_peer = peer;

		_data_size = 8 * 1024;
		_data = (unsigned char*)malloc(_data_size);
	}

	virtual ~enetchannel(){
		free(_data);
	}

	void Init() {
		ch_encrypt_decrypt_ondata = std::make_shared<channel_encrypt_decrypt_ondata>(shared_from_this());
	}

public:
	void disconnect()
	{
	}

	void recv(char * data, size_t length)
	{
		ch_encrypt_decrypt_ondata->recv(data, length);
	}

	void send(std::string& data)
	{
		try {
			auto len = data.size();
			if (_data_size < (len + 4)) {
				_data_size *= 2;
				free(_data);
				_data = (unsigned char*)malloc(_data_size);
			}
			_data[0] = len & 0xff;
			_data[1] = len >> 8 & 0xff;
			_data[2] = len >> 16 & 0xff;
			_data[3] = len >> 24 & 0xff;
			memcpy(&_data[4], data.c_str(), len);
			size_t datasize = len + 4;

			ENetPacket* packet = enet_packet_create(_data, datasize, ENET_PACKET_FLAG_RELIABLE);
			enet_peer_send(_peer, 0, packet);
			enet_host_flush(_host);
			enet_packet_destroy(packet);
		}
		catch (std::exception e) {
			spdlog::error("enetchannel push exception error:{0}", e.what());
		}
	}

private:
	ENetHost * _host;
	ENetPeer * _peer;

	unsigned char* _data;
	size_t _data_size;

	std::shared_ptr<channel_encrypt_decrypt_ondata> ch_encrypt_decrypt_ondata;

};

}

#endif
