using System;

namespace center
{
	class svrproxy
	{
		public svrproxy(juggle.Ichannel ch)
		{
			caller = new caller.center_call_server(ch);
		}

		public void reg_server_sucess()
		{
			caller.reg_server_sucess();
		}

		public void close_server()
		{
			caller.close_server();
		}

		private caller.center_call_server caller;
	}
}
