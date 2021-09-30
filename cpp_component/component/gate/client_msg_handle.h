/*
 * client_msg_handle.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _client_msg_handle_h
#define _client_msg_handle_h
//
//#include <gate_call_hubcaller.h>
//#include <gate_call_clientcaller.h>
//
//#include "hubsvrmanager.h"
//#include "clientmanager.h"
//
//void connect_server(std::shared_ptr<gate::hubsvrmanager> _hubsvrmanager, std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<service::timerservice> _timerservice, std::string uuid, int64_t clienttick) {
//	if (_clientmanager->has_client(uuid)){
//		return;
//	}
//
//	std::cout << "client connect " << uuid << std::endl;
//
//	//_hubsvrmanager->for_each_hub([uuid](std::string hub_name, std::shared_ptr<juggle::Ichannel> ch) {
//	//	auto _hubproxy = std::make_shared<caller::gate_call_hub>(ch);
//	//	_hubproxy->client_connect(uuid);
//	//});
//
//	_clientmanager->reg_client(uuid, juggle::current_ch, _timerservice->Tick, clienttick);
//	auto _client_proxy = std::make_shared<caller::gate_call_client>(juggle::current_ch);
//	_client_proxy->connect_gate_sucess();
//}
//
//void cancle_server(std::shared_ptr<gate::clientmanager> _clientmanager) {
//	_clientmanager->unreg_client(juggle::current_ch);
//}
//
//void connect_hub(std::shared_ptr<gate::hubsvrmanager> _hubsvrmanager, std::shared_ptr<gate::clientmanager> _clientmanager, std::string hub_Name) {
//	if (!_clientmanager->has_client(juggle::current_ch)) {
//		return;
//	}
//	auto client_uuid = _clientmanager->get_client(juggle::current_ch);
//	auto _hub_ch = _hubsvrmanager->get_hub(hub_Name);
//	if (_hub_ch == nullptr) {
//		return;
//	}
//	auto _hub_proxy = std::make_shared<caller::gate_call_hub>(_hub_ch);
//	_hub_proxy->client_connect(client_uuid);
//}
//
//void enable_heartbeats(std::shared_ptr<gate::clientmanager> _clientmanager)
//{
//	_clientmanager->enable_heartbeats(juggle::current_ch);
//}
//
//void disable_heartbeats(std::shared_ptr<gate::clientmanager> _clientmanager)
//{
//	_clientmanager->disable_heartbeats(juggle::current_ch);
//}
//
//void forward_client_call_hub(std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<gate::hubsvrmanager> _hubsvrmanager, std::string hub_name, std::string module_name, std::string func_name, std::shared_ptr<std::vector<boost::any> > argv) {
//	if (!_clientmanager->has_client(juggle::current_ch)){
//		return;
//	}
//	auto uuid = _clientmanager->get_client(juggle::current_ch);
//
//	auto _hub_channel = _hubsvrmanager->get_hub(hub_name);
//	if (_hub_channel == nullptr){
//		return;
//	}
//
//	auto _hub_proxy = std::make_shared<caller::gate_call_hub>(_hub_channel);
//	_hub_proxy->client_call_hub(uuid, module_name, func_name, argv);
//}
//
//void heartbeats(std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<service::timerservice> _timerservice, std::int64_t clienttick) {
//	if (_clientmanager->has_client(juggle::current_ch)) {
//		_clientmanager->refresh_and_check_client(juggle::current_ch, _timerservice->Tick, clienttick);
//
//		auto _caller = std::make_shared<caller::gate_call_client>(juggle::current_ch);
//		_caller->ack_heartbeats();
//	}
//}

#endif //_client_msg_handle_h
