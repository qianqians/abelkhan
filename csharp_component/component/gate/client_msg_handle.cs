
namespace Gate {

	public class client_msg_handle {
		private readonly ClientManager _clientmanager;
		private readonly HubSvrManager _hubsvrmanager;

		private readonly Abelkhan.client_call_gate_module _client_call_gate_module;

		public client_msg_handle(ClientManager clientmanager_, HubSvrManager hubsvrmanager_) {
			_clientmanager = clientmanager_;
			_hubsvrmanager = hubsvrmanager_;

			_client_call_gate_module = new Abelkhan.client_call_gate_module(Abelkhan.ModuleMgrHandle._modulemng);
			_client_call_gate_module.on_heartbeats += heartbeats;
			_client_call_gate_module.on_get_hub_info += get_hub_info;
			_client_call_gate_module.on_forward_client_call_hub += forward_client_call_hub;
            _client_call_gate_module.on_migrate_client_confirm += _client_call_gate_module_on_migrate_client_confirm;

        }

		private void _client_call_gate_module_on_migrate_client_confirm(string src_hub, string _target_hub)
		{
			var ch = _client_call_gate_module.current_ch.Value;
			if (_clientmanager.get_client(ch, out var proxy) && _hubsvrmanager.get_hub(src_hub, out var hubproxy_))
			{
                hubproxy_.migrate_client(proxy._cuuid, _target_hub);
            }
		}

        private void heartbeats() {
			var rsp = (Abelkhan.client_call_gate_heartbeats_rsp)_client_call_gate_module.rsp.Value;
			var ch = _client_call_gate_module.current_ch.Value;
			if (_clientmanager.get_client(ch, out var proxy)) {
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
            var ch = _client_call_gate_module.current_ch.Value;
            if (_clientmanager.get_client(ch, out var proxy) && _hubsvrmanager.get_hub_list(proxy._cuuid, hub_type, out var _info)) {
				rsp.rsp(_info);
			}
			else {
				rsp.err();
			}
		}

		private void forward_client_call_hub(string hub_name, byte[] rpc_argv) {
			var ch = _client_call_gate_module.current_ch.Value;
			if (_clientmanager.get_client(ch, out var proxy)) {
                if  (_hubsvrmanager.get_hub(hub_name, out var hubproxy_))
				{
					proxy.conn_hub(hubproxy_);
					hubproxy_.client_call_hub(proxy._cuuid, rpc_argv);

					if (hubproxy_._tick_time > 100)
					{
						var r = _hubsvrmanager.rd.Next(100);
						if (r < 20)
						{
							var target_hub = _hubsvrmanager.hash_hubproxy(proxy._cuuid, hubproxy_._hub_type);
							if (_hubsvrmanager.get_hub(target_hub, out var target_hubproxy_) && target_hub != hub_name && target_hubproxy_._tick_time <= 50)
							{
								proxy.migrate_client_start(hub_name, target_hub);
							}
						}
					}
				}
				else
				{
					proxy.hub_loss(hub_name);
                }
			}
		}
	}

}