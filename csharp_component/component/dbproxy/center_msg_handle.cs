﻿using System;

namespace DBProxy
{
	public class center_msg_handle
	{
		private CloseHandle _closehandle;
		private CenterProxy _centerproxy;
        private HubManager _hubs;

		private Abelkhan.center_call_server_module _center_call_server_module;

		public center_msg_handle(CloseHandle _closehandle_, CenterProxy _centerproxy_, HubManager _hubs_)
		{
			_closehandle = _closehandle_;
            _centerproxy = _centerproxy_;
            _hubs = _hubs_;

			_center_call_server_module = new Abelkhan.center_call_server_module(Abelkhan.ModuleMgrHandle._modulemng);
			_center_call_server_module.on_close_server += close_server;
			_center_call_server_module.on_console_close_server += console_close_server;
			_center_call_server_module.on_svr_be_closed += svr_be_closed;
			_center_call_server_module.on_take_over_svr += take_over_svr;

        }

		private void close_server()
		{
			_closehandle._is_closing = true;
			check_close_server();
        }

        private void close_server_impl(long tick)
        {
            _closehandle._is_close = true;
        }

		private void check_close_server()
		{
            if (_closehandle._is_closing && _hubs.all_hub_closed())
            {
				_centerproxy.closed();
                DBProxy._timer.addticktime(3000, close_server_impl);
            }
        }

		private void console_close_server(string svr_type, string svr_name)
        {
			if (svr_type == "dbproxy" && svr_name == DBProxy.name)
            {
				DBProxy._timer.addticktime(3000, close_server_impl);
			}
            else
            {
				if (svr_type == "hub")
				{
					_hubs.on_hub_closed(svr_name);
                    check_close_server();
                }
			}
        }

		private void svr_be_closed(string svr_type, string svr_name)
        {
            Log.Log.trace("svr_be_closed");

			if (svr_type == "hub")
			{
				_hubs.on_hub_closed(svr_name);
                check_close_server();
            }
		}

		private void take_over_svr(string svr_name)
		{
			DBProxy._redis_mq_service.take_over_svr(svr_name);
        }
    }
}

