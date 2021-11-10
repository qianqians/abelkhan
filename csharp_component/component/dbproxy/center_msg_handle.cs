using System;

namespace dbproxy
{
	public class center_msg_handle
	{
		private closehandle _closehandle;
		private hubmanager _hubs;

		private abelkhan.center_call_server_module _center_call_server_module;

		public center_msg_handle(closehandle _closehandle_, hubmanager _hubs_)
		{
			_closehandle = _closehandle_;
			_hubs = _hubs_;

			_center_call_server_module = new abelkhan.center_call_server_module(abelkhan.modulemng_handle._modulemng);
			_center_call_server_module.on_close_server += close_server;
			_center_call_server_module.on_svr_be_closed += svr_be_closed;
		}

		public void close_server()
		{
            dbproxy._timer.addticktime(3 * 1000, close_server_impl);
		}

        private void close_server_impl(Int64 tick)
        {
            _closehandle._is_close = true;
        }

		public void svr_be_closed(string svr_type, string svr_name)
        {
            log.log.trace("svr_be_closed");

			if (svr_type == "hub")
			{
				_hubs.on_hub_closed(svr_name);
			}
		}


	}
}

