using System;

namespace dbproxy
{
	public class center_msg_handle
	{
		private closehandle _closehandle;
		private centerproxy _centerproxy;
        private hubmanager _hubs;

		private abelkhan.center_call_server_module _center_call_server_module;

		public center_msg_handle(closehandle _closehandle_, centerproxy _centerproxy_, hubmanager _hubs_)
		{
			_closehandle = _closehandle_;
            _centerproxy = _centerproxy_;
            _hubs = _hubs_;

			_center_call_server_module = new abelkhan.center_call_server_module(abelkhan.modulemng_handle._modulemng);
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
                dbproxy._timer.addticktime(3000, close_server_impl);
            }
        }

		private void console_close_server(string svr_type, string svr_name)
        {
			if (svr_type == "dbproxy" && svr_name == dbproxy.name)
            {
				dbproxy._timer.addticktime(3000, close_server_impl);
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
            log.log.trace("svr_be_closed");

			if (svr_type == "hub")
			{
				_hubs.on_hub_closed(svr_name);
                check_close_server();
            }
		}

		private void take_over_svr(string svr_name)
		{
			dbproxy._redis_mq_service.take_over_svr(svr_name);
        }
    }
}

