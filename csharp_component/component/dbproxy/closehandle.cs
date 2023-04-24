using System;

namespace DBProxy
{
	public class CloseHandle
	{
		public CloseHandle()
		{
			_is_close = false;
            _is_closing = false;
        }

		public bool is_close(){
			return DBProxy._hubmanager.all_hub_closed() && _is_close;
		}

		public bool _is_close;
		public bool _is_closing;
	}
}

