using System;

namespace center
{
	class hubproxy
	{
		public hubproxy (juggle.Ichannel _ch, int _zone_id)
		{
			_caller = new caller.center_call_hub(_ch);

            zone_id = _zone_id;
        }

		public void distribute_server_address(String type, String ip, Int64 port, String uuid)
		{
			_caller.distribute_server_address(type, ip, port, uuid);
		}

        public void reload(String argv)
        {
            _caller.reload(argv);
        }

        public int zone_id;
        public bool is_closed = false;

		private caller.center_call_hub _caller;
	}
}
