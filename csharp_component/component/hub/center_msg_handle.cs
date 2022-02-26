using System;

namespace hub
{
	public class center_msg_handle
	{
		private hub _hub;
		private closehandle _closehandle;
		private centerproxy _centerproxy;
		private abelkhan.center_call_server_module _center_call_server_module;
		private abelkhan.center_call_hub_module _center_call_hub_module;

		public center_msg_handle(hub _hub_, closehandle _closehandle_, centerproxy _centerproxy_)
		{
			_hub = _hub_;
			_closehandle = _closehandle_;
			_centerproxy = _centerproxy_;

			_center_call_server_module = new abelkhan.center_call_server_module(abelkhan.modulemng_handle._modulemng);
			_center_call_server_module.on_close_server += close_server;
			_center_call_server_module.on_svr_be_closed += svr_be_closed;

			_center_call_hub_module = new abelkhan.center_call_hub_module(abelkhan.modulemng_handle._modulemng);
			_center_call_hub_module.on_distribute_server_address += distribute_server_address;
			_center_call_hub_module.on_reload += reload;
		}

		public void close_server()
		{
            _hub.onCloseServer_event();
		}

		public void svr_be_closed(string svr_type, string svr_name)
        {
			if (svr_type == "dbproxy")
            {
				log.log.err("dbproxy exception closed!");
            }
			else if (svr_type == "gate")
            {
				hub._gates.gate_be_closed(svr_name);
            }
            else if (svr_type == "hub")
			{
				hub._hubs.hub_be_closed(svr_name);
			}
        }

		public void distribute_server_address(String type, String name, String host, ushort port)
        {
            log.log.trace("recv distribute server address");

			if (type == "dbproxy") 
			{
				log.log.trace("recv distribute server address connect_dbproxy ip:{0}, port:{1}", host, port);
				_hub.connect_dbproxy (name, host, (short)port);
			}
			if (type == "gate")
			{
				log.log.trace("recv distribute server address gate ip:{0}, port:{1}", host, port);
				hub._gates.connect_gate(name, host, (ushort)port);
			}
            if (type == "hub")
			{
				log.log.trace("recv distribute server address hub ip:{0}, port:{1}", host, port);
				_hub.reg_hub(host, (short)port);
            }
		}

		public void reload(string argv)
        {
			_hub.onReload_event(argv);
		}
	}
}

