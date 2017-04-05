using System;

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
			_closehandle._is_close = true;
		}

		public void reg_server_sucess()
		{
			Console.WriteLine("connect center server sucess");

			_centerproxy.is_reg_sucess = true;
		}
			
		private closehandle _closehandle;
		private centerproxy _centerproxy;


	}
}

