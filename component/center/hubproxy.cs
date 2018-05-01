using System;

namespace center
{
	class hubproxy
	{
		public hubproxy (juggle.Ichannel _ch)
		{
			_caller = new caller.center_call_hub(_ch);
		}

		public void distribute_server_address(String type, String ip, Int64 port, String uuid)
		{
			_caller.distribute_server_address(type, ip, port, uuid);
		}

        public void reload(String argv)
        {
            _caller.reload(argv);
        }

        public bool is_closed = false;

		private caller.center_call_hub _caller;
	}
}
