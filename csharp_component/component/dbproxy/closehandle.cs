using System;

namespace dbproxy
{
	public class closehandle
	{
		public closehandle()
		{
			_is_close = false;
		}

		public bool is_close(){
			return (dbproxy._hubmanager.hub_num() <= 0) && _is_close;
		}

		public bool _is_close;
	}
}

