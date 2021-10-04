﻿using System;

namespace dbproxy
{
	public class center_msg_handle
	{
		public center_msg_handle(closehandle _closehandle_, centerproxy _centerproxy_)
		{
			_closehandle = _closehandle_;
			_centerproxy = _centerproxy_;
		}

		public void close_server()
		{
            dbproxy.timer.addticktime(3 * 1000, close_server_impl);
		}

        private void close_server_impl(Int64 tick)
        {
            _closehandle._is_close = true;
        }

		public void reg_server_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "connect center server sucess");

			_centerproxy.is_reg_sucess = true;
		}
			
		private closehandle _closehandle;
		private centerproxy _centerproxy;


	}
}

