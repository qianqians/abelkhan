using Abelkhan;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gate {

	public class hub_svr_msg_handle {
		private readonly ClientManager _clientmanager;
		private readonly HubSvrManager _hubsvrmanager;

		private readonly Abelkhan.hub_call_gate_module _hub_call_gate_module;

		public hub_svr_msg_handle(ClientManager clientmanager_, HubSvrManager hubsvrmanager_) {
			_clientmanager = clientmanager_;
			_hubsvrmanager = hubsvrmanager_;

			_hub_call_gate_module = new Abelkhan.hub_call_gate_module(Abelkhan.ModuleMgrHandle._modulemng);
			_hub_call_gate_module.on_reg_hub += reg_hub;
			_hub_call_gate_module.on_tick_hub_health += tick_hub_health;
			_hub_call_gate_module.on_reverse_reg_client_hub += reverse_reg_client_hub;
			_hub_call_gate_module.on_unreg_client_hub += unreg_client_hub;
			_hub_call_gate_module.on_disconnect_client += disconnect_client;
            _hub_call_gate_module.on_migrate_client_done += _hub_call_gate_module_on_migrate_client_done;
            _hub_call_gate_module.on_forward_hub_call_client += forward_hub_call_client;
			_hub_call_gate_module.on_forward_hub_call_group_client += forward_hub_call_group_client;
			_hub_call_gate_module.on_forward_hub_call_global_client += forward_hub_call_global_client;
		}

        private void _hub_call_gate_module_on_migrate_client_done(string client_uuid, string _src_hub, string _target_hub)
        {
            if (_clientmanager.get_client(client_uuid, out var client_proxy))
            {
                client_proxy.migrate_client_done(_src_hub, _target_hub);
            }
            else
            {
                var ch = _hub_call_gate_module.current_ch.Value;
				if (_hubsvrmanager.get_hub(ch, out var hub_proxy))
				{
					hub_proxy.client_disconnect(client_uuid);
				}
            }
        }

        public void reg_hub(string hub_name, string hub_type, string router_type) {
			var rsp = (Abelkhan.hub_call_gate_reg_hub_rsp)_hub_call_gate_module.rsp.Value;
			var ch = _hub_call_gate_module.current_ch.Value;
			_ = _hubsvrmanager.reg_hub(hub_name, hub_type, router_type, ch);
			rsp.rsp();
		}

		public void tick_hub_health(uint tick_time) {
			if (_hubsvrmanager.get_hub(_hub_call_gate_module.current_ch.Value, out var hub_proxy)) {
				hub_proxy._tick_time = tick_time;
			}
		}

		public void reverse_reg_client_hub(string client_uuid) {
			var rsp = (Abelkhan.hub_call_gate_reverse_reg_client_hub_rsp)_hub_call_gate_module.rsp.Value;
			if (_clientmanager.get_client(client_uuid, out var client_proxy) && _hubsvrmanager.get_hub(_hub_call_gate_module.current_ch.Value, out var hub_proxy)) {
                client_proxy.conn_hub(hub_proxy);
				rsp.rsp();
			}
			else {
				rsp.err(Abelkhan.framework_error.enum_framework_client_not_exist);
			}
		}

		public void unreg_client_hub(string client_uuid) {
			if (_clientmanager.get_client(client_uuid, out var proxy) && _hubsvrmanager.get_hub(_hub_call_gate_module.current_ch.Value, out var hub_proxy)) {
				proxy.unreg_hub(hub_proxy);
			}
		}

		public void disconnect_client(string cuuid, string reason) {
			if (_clientmanager.get_client(cuuid, out var proxy)) {
                proxy.ntf_reason(reason);
                proxy._ch.disconnect();
			}
		}

		public void forward_hub_call_client(string cuuid, byte[] rpc_argv) {
			var ch = _hub_call_gate_module.current_ch.Value;
			if (_hubsvrmanager.get_hub(ch, out var hub_proxy))
            {
                if (_clientmanager.get_client(cuuid, out var client_proxy))
                {
                    client_proxy.conn_hub(hub_proxy);
                    client_proxy.call_client(hub_proxy._hub_name, rpc_argv);
                }
                else
                {
                    hub_proxy.client_disconnect(cuuid);
                }
            }
		}

		public void forward_hub_call_group_client(List<string> cuuids, byte[] rpc_argv) {
			var ch = _hub_call_gate_module.current_ch.Value;
			if (_hubsvrmanager.get_hub(ch, out var hub_proxy))
			{
				var clients = new List<ClientProxy>();
				foreach (var cuuid in cuuids)
				{
					if (_clientmanager.get_client(cuuid, out var client_proxy))
					{
						clients.Add(client_proxy);
					}
					else
					{
						hub_proxy.client_disconnect(cuuid);
					}
				}

				_ = Parallel.ForEach(clients, client_proxy =>
                {
                    client_proxy.conn_hub(hub_proxy);
                    client_proxy.call_client(hub_proxy._hub_name, rpc_argv);
				});
			}
        }

		public void forward_hub_call_global_client(byte[] rpc_argv) {
			var ch = _hub_call_gate_module.current_ch.Value;
			if (_hubsvrmanager.get_hub(ch, out var hub_proxy))
			{
				_clientmanager.for_each_client((_, _client) =>
				{
					_client.conn_hub(hub_proxy);
					_client.call_client(hub_proxy._hub_name, rpc_argv);
				});
			}
		}
	}

}
