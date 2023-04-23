﻿using abelkhan;
using System;

namespace hub
{
	public class center_msg_handle
	{
		private readonly Hub _hub;
		private readonly Closehandle _closehandle;
		private readonly Centerproxy _centerproxy;
		private readonly abelkhan.center_call_server_module _center_call_server_module;
		private readonly abelkhan.center_call_hub_module _center_call_hub_module;

		public center_msg_handle(Hub _hub_, Closehandle _closehandle_, Centerproxy _centerproxy_)
		{
			_hub = _hub_;
			_closehandle = _closehandle_;
			_centerproxy = _centerproxy_;

			_center_call_server_module = new abelkhan.center_call_server_module(abelkhan.modulemng_handle._modulemng);
			_center_call_server_module.on_close_server += close_server;
			_center_call_server_module.on_console_close_server += console_close_server;
			_center_call_server_module.on_svr_be_closed += svr_be_closed;
			_center_call_server_module.on_take_over_svr += take_over_svr;

            _center_call_hub_module = new abelkhan.center_call_hub_module(abelkhan.modulemng_handle._modulemng);
            _center_call_hub_module.on_distribute_server_mq += distribute_server_mq;
			_center_call_hub_module.on_reload += reload;
		}

        private void close_server()
		{
            _hub.onCloseServer_event();
		}

		private void console_close_server(string svr_type, string svr_name)
		{
			if (svr_type == "hub" && svr_name == Hub.name)
			{
				_hub.onCloseServer_event();
			}
			else
			{
				svr_be_closed(svr_type, svr_name);
			}
		}

		private void svr_be_closed(string svr_type, string svr_name)
        {
			if (svr_type == "dbproxy")
            {
				log.Log.err("dbproxy exception closed!");

            }
			else if (svr_type == "gate")
            {
				Hub._gates.gate_be_closed(svr_name);
            }
            else if (svr_type == "hub")
			{
				Hub._hubs.hub_be_closed(svr_name);
			}
        }

		private void take_over_svr(string svr_name)
		{
			if (Hub.is_support_take_over_svr)
            {
                Hub._redis_mq_service.take_over_svr(svr_name);
            }
		}

        private void distribute_server_mq(String type, String name)
		{
			log.Log.trace("recv distribute server address");

			if (type == "dbproxy")
			{
				log.Log.trace("recv distribute server address connect_dbproxy name:{0}", name);
				_hub.connect_dbproxy(name);
			}
			if (type == "gate")
			{
				log.Log.trace("recv distribute server address gate name:{0}", name);
				Hub._gates.connect_gate(name);
			}
			if (type == "hub")
			{
				log.Log.trace("recv distribute server address hub name:{0}", name);
				_hub.reg_hub(name);
			}
		}

		private void reload(string argv)
        {
			_hub.onReload_event(argv);
		}
	}
}

