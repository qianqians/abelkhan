#ifndef _enetchannel_h
#define _enetchannel_h

#include <list>
#include <iostream>
#include <mutex>

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
	}

	virtual ~enetchannel(){
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

	void send(char* data, size_t len)
	{
		try {
			ENetPacket* packet = enet_packet_create(data, len, ENET_PACKET_FLAG_RELIABLE);

			std::lock_guard<std::mutex> lock(_mutex);
			enet_peer_send(_peer, 0, packet);
			enet_host_flush(_host);
		}
		catch (std::exception e) {
			spdlog::error("enetchannel push exception error:{0}", e.what());
		}
	}

private:
	ENetHost * _host;
	ENetPeer * _peer;

	std::mutex _mutex;

	std::shared_ptr<channel_encrypt_decrypt_ondata> ch_encrypt_decrypt_ondata;

};

}

#endif
