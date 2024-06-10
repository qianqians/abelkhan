

using Amazon.Runtime.Internal.Util;

namespace Gate {

	public class client_msg_handle {
		private ClientManager _clientmanager;
		private HubSvrManager _hubsvrmanager;
		private Service.Timerservice _timerservice;

		private Abelkhan.client_call_gate_module _client_call_gate_module;

		public client_msg_handle(ClientManager clientmanager_, HubSvrManager hubsvrmanager_, Service.Timerservice timerservice_) {
			_clientmanager = clientmanager_;
			_hubsvrmanager = hubsvrmanager_;
			_timerservice = timerservice_;

			_client_call_gate_module = new Abelkhan.client_call_gate_module(Abelkhan.ModuleMgrHandle._modulemng);
			_client_call_gate_module.on_heartbeats += heartbeats;
			_client_call_gate_module.on_get_hub_info += get_hub_info;
			_client_call_gate_module.on_forward_client_call_hub += forward_client_call_hub;
		}

		private void heartbeats() {
			var rsp = (Abelkhan.client_call_gate_heartbeats_rsp)_client_call_gate_module.rsp.Value;
			var ch = _client_call_gate_module.current_ch.Value;
			var proxy = _clientmanager.get_client(ch);
			if (proxy != null) {
				if (proxy._theory_timetmp == 0) {
					proxy._theory_timetmp = Service.Timerservice.Tick;
				}
				else {
					proxy._theory_timetmp += 5000;
				}
				proxy._timetmp = Service.Timerservice.Tick;
			}
			rsp.rsp((ulong)Service.Timerservice.Tick);
		}

		private void get_hub_info(string hub_type) {
			var rsp = (Abelkhan.client_call_gate_get_hub_info_rsp)_client_call_gate_module.rsp.Value;
			if (_hubsvrmanager.get_hub_list(hub_type, out var _info)) {
				rsp.rsp(_info);
			}
			else {
				rsp.err();
			}
		}

		private void forward_client_call_hub(string hub_name, byte[] rpc_argv) {
			var ch = _client_call_gate_module.current_ch.Value;
			var proxy = _clientmanager.get_client(ch);
			if (proxy != null) {
				var hubproxy_ = _hubsvrmanager.get_hub(hub_name);
				if (hubproxy_ != null) {
					proxy.conn_hub(hubproxy_);
					hubproxy_.client_call_hub(proxy._cuuid, rpc_argv);
				}
			}
		}
	}

}