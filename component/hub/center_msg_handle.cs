using System;

namespace hub
{
	public class center_msg_handle
	{
		public center_msg_handle(hub _hub_, closehandle _closehandle_, centerproxy _centerproxy_)
		{
			_hub = _hub_;
			_closehandle = _closehandle_;
			_centerproxy = _centerproxy_;
		}

		public void reg_server_sucess()
		{
			Console.WriteLine("connect center server sucess");
			_centerproxy.is_reg_center_sucess = true;
		}

		public void close_server()
		{
			_closehandle.is_close = true;
		}

		public void distribute_server_address(String type, String ip, Int64 port, String uuid)
		{
			Console.WriteLine("recv distribute server address");
			if (type == "dbproxy") 
			{
				_hub.connect_dbproxy (ip, (short)port);
			}
			if (type == "gate") 
			{
				hub.gates.connect_gate(uuid, ip, (short)port);
			}
            if (type == "hub")
            {
                _hub.reg_hub(ip, (short)port);
            }
		}

		private hub _hub;
		private closehandle _closehandle;
		private centerproxy _centerproxy;
	}
}

