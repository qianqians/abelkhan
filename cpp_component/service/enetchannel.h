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

	void set_xor_key_crypt() {
		ch_encrypt_decrypt_ondata->set_xor_key_crypt();
	}

	bool is_xor_key_crypt() {
		return ch_encrypt_decrypt_ondata->is_compress_and_encrypt;
	}

	void normal_crypt(char* data, size_t len) {
		ch_encrypt_decrypt_ondata->xor_key_encrypt_decrypt(data, len);
	}

	void recv(char * data, size_t length)
	{
		ch_encrypt_decrypt_ondata->recv(data, length);
	}

	void send(const char* data, size_t len)
	{
		try {
			ENetPacket* packet = enet_packet_create(data, len, ENET_PACKET_FLAG_RELIABLE);

			std::lock_guard<std::mutex> lock(_mutex);
			enet_peer_send(_peer, 0, packet);
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
