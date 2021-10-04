using System;

namespace dbproxy
{
	public class closehandle
	{
		public closehandle()
		{
			_is_close = false;
		}

		public void reg_logic() {
			online_logic_num += 1;
		}

		public void logic_closed() {
			online_logic_num -= 1;
		}

		public bool is_close(){
			return (online_logic_num == 0) && _is_close;
		}

		public bool _is_close;

		private int online_logic_num;
	}
}

