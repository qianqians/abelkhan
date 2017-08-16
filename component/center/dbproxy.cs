using System;

namespace center
{
	class dbproxy
	{
		public dbproxy (juggle.Ichannel _ch_, String _type, String _ip, Int64 _port, String _uuid)
		{
			_ch = _ch_;
			type = _type;
			ip = _ip;
			port = _port;
			uuid = _uuid;
		}

		public juggle.Ichannel _ch;
		public String type;
		public String ip;
		public Int64 port;
		public String uuid;
	}
}
