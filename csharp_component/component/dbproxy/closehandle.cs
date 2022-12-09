using System;

namespace dbproxy
{
	public class closehandle
	{
		public closehandle()
		{
			_is_close = false;
            _is_closing = false;
        }

		public bool is_close(){
			return dbproxy._hubmanager.all_hub_closed() && _is_close;
		}

		public bool _is_close;
		public bool _is_closing;
	}
}

