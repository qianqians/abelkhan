using System;

namespace logic
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
			_closehandle.is_close = true;
		}

		public void reg_server_sucess()
		{
			Console.WriteLine("connect center server sucess");
			_centerproxy.is_reg_sucess = true;
		}

		public void distribute_server_address(String type, String ip, Int64 port, String uuid)
		{
			if (type == "dbproxy")
			{
				logic.dbproxy.connect(ip, (short)port);
				logic.dbproxy.reg_logic(logic.uuid);
			}
			else if (type == "hub")
			{
				logic.hubs.connect_hub(ip, (short)port);
			}
			else if (type == "logic")
			{
				logic.logics.reg_logic(uuid, ip, (short)port);
			}
			else if (type == "gate")
			{
				logic.gates.connect_gate(uuid, ip, (short)port);
			}
		}

		public void ack_get_server_address(Boolean has_svr, String type, String ip, Int64 port, String uuid, String callbackid)
		{
			if (type == "hub")
			{
				logic.hubs.connect_hub(ip, (short)port);
			}
			else if (type == "logic")
			{
				logic.logics.reg_logic(uuid, ip, (short)port);
			}
			else if (type == "gate")
			{
				logic.gates.connect_gate(uuid, ip, (short)port);
			}
		}

		private closehandle _closehandle;
		private centerproxy _centerproxy;


	}
}

