using System;

namespace gate
{
	public class center_msg_handle
	{
		public center_msg_handle(closehandle _closehandle_, centerproxy _centerproxy_)
		{
			_closehandle = _closehandle_;
			_centerproxy = _centerproxy_;
		}

		public void reg_server_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "connect center server sucess");

			_centerproxy.is_reg_center_sucess = true;
		}

		public void close_server()
		{
			_closehandle.is_close = true;
		}

		private closehandle _closehandle;
		private centerproxy _centerproxy;
	}
}

