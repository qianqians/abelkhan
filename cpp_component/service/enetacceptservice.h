#ifndef _enetacceptservice_h
#define _enetacceptservice_h

#include <unordered_map>

#include <concurrent/signals.h>

#include "DNS.h"
#include "enetchannel.h"

namespace service
{

class enetacceptservice {
public:
	enetacceptservice(std::string host, short port){
		ENetAddress address;
		if (enet_address_set_host_ip(&address, "0.0.0.0") != 0) {
			throw std::exception("enet_address_set_host_ip faild");
			return;
		}
		address.port = port;
		_host = enet_host_create(&address, 2048, 1, 0, 0);
	}

	void poll()
	{
		ENetEvent _event;
		if (enet_host_service(_host, &_event, 5) > 0) {
			switch (_event.type)
			{
			case ENET_EVENT_TYPE_CONNECT:
			{
				spdlog::trace("enetacceptservice connect begin!");

				std::shared_ptr<enetchannel> ch = nullptr;
				uint64_t peerHandle = (uint64_t)_event.peer->address.host << 32 | _event.peer->address.port;

				auto it_ch = back_chs.find(peerHandle);
				if (it_ch == back_chs.end()) {
					ch = std::make_shared<enetchannel>(_host, _event.peer);
					ch->Init();
					back_chs.insert(std::make_pair(peerHandle, ch));

					sig_connect.emit(ch);
				}
				else {
					ch = it_ch->second;
					back_chs.erase(it_ch);
				}

				if (chs.find(peerHandle) == chs.end()) {
					chs.insert(std::make_pair(peerHandle, ch));
				}
				else {
					chs[peerHandle] = ch;
				}

				char ip[256] = {0};
				enet_address_get_host_ip(&_event.peer->address, ip, 256);
				std::string cb_handle = std::string(ip) + ":" + std::to_string(_event.peer->address.port);
				auto cb = cbs.find(cb_handle);
				if (cb != cbs.end()) {
					spdlog::trace("enetacceptservice cb_handle:{0}", cb_handle);
					(cb->second)(ch);
					cbs.erase(cb_handle);
				}

				spdlog::trace("enetacceptservice connect end!");
			}
			break;
			case ENET_EVENT_TYPE_RECEIVE:
			{
				spdlog::trace("enetacceptservice recv begin!");

				uint64_t peerHandle = (uint64_t)_event.peer->address.host << 32 | _event.peer->address.port;
				chs[peerHandle]->recv((char*)_event.packet->data, _event.packet->dataLength);

				enet_packet_destroy(_event.packet);

				spdlog::trace("enetacceptservice recv end!");
			}
			break;
			case ENET_EVENT_TYPE_DISCONNECT:
			{
				uint64_t peerHandle = (uint64_t)_event.peer->address.host << 32 | _event.peer->address.port;
				auto it = chs.find(peerHandle);
				if (it != chs.end()) {
					sig_disconnect.emit(it->second);
					chs.erase(it);
				}
			}
			break;
			default:
				break;
			}
		}
		enet_host_flush(_host);
	}

	void connect(std::string hub_host, short port, std::function<void(std::shared_ptr<abelkhan::Ichannel>)> cb)
	{
		auto ip = service::DNS(hub_host);
		ENetAddress address;
		if (enet_address_set_host_ip(&address, ip.c_str()) != 0) {
			throw std::exception("enet_address_set_host_ip faild");
		}
		address.port = port;

		ENetPeer* peer = enet_host_connect(_host, &address, 1, 0);
		if (peer == nullptr) {
			throw std::exception("enet_host_connect faild");
		}

		std::string cb_handle = ip + ":" + std::to_string(port);
		cbs.insert(std::make_pair(cb_handle, cb));
	}

public:
	concurrent::signals<void(std::shared_ptr<abelkhan::Ichannel>)> sig_connect;
	concurrent::signals<void(std::shared_ptr<abelkhan::Ichannel>)> sig_disconnect;

private:
	ENetHost * _host;

	std::unordered_map<std::string, std::function<void(std::shared_ptr<abelkhan::Ichannel>)> > cbs;
	std::unordered_map<uint64_t, std::shared_ptr<enetchannel> > back_chs;
	std::unordered_map<uint64_t, std::shared_ptr<enetchannel> > chs;

};

}

#endif //_enetacceptservice_h
