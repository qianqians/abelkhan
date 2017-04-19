using System;
using System.Collections;

namespace dbproxy
{
	public class hub_msg_handle
	{
		public hub_msg_handle(hubmanager _hubmanager_, mongodbproxy _mongodbproxy_)
		{
			_hubmanager = _hubmanager_;
			_mongodbproxy = _mongodbproxy_;
		}

		public void reg_hub(string uuid)
		{
			System.Console.WriteLine ("hub " + uuid + " connected");

			hubproxy _hubproxy = _hubmanager.reg_hub(juggle.Imodule.current_ch, uuid);
			_hubproxy.reg_hub_sucess ();
		}

		public void create_persisted_object(Hashtable object_info, string callbackid)
		{
			_mongodbproxy.save(object_info);

			hubproxy _hubproxy = _hubmanager.get_hub(juggle.Imodule.current_ch);
			if (_hubproxy != null)
			{
				_hubproxy.ack_create_persisted_object (callbackid);
			}
		}

		public void updata_persisted_object(Hashtable query_json, Hashtable object_info, string callbackid)
		{
			_mongodbproxy.update(query_json, object_info);

			hubproxy _hubproxy = _hubmanager.get_hub(juggle.Imodule.current_ch);
			if (_hubproxy != null) 
			{
				_hubproxy.ack_updata_persisted_object (callbackid);
			}
		}

		public void get_object_info(Hashtable query_json, string callbackid)
		{
            Console.WriteLine("get_object_info");

            ArrayList _list = _mongodbproxy.find(0, 0, 0, query_json, null);

			hubproxy _hubproxy = _hubmanager.get_hub(juggle.Imodule.current_ch);

			if (_hubproxy == null) 
			{
                Console.WriteLine("get_object_info hubproxy is null");
				return;
			}

			int count = 0;
			ArrayList _datalist = new ArrayList ();
            if (_list.Count == 0)
            {
                _hubproxy.ack_get_object_info(callbackid, _datalist);
            }
            else
            {
                foreach (var data in _list)
                {
                    _datalist.Add(data);

                    count++;

                    if (count >= 100)
                    {
                        _hubproxy.ack_get_object_info(callbackid, _datalist);

                        count = 0;
                        _datalist = new ArrayList();
                    }
                }
                if (count > 0 && count < 100)
                {
                    _hubproxy.ack_get_object_info(callbackid, _datalist);
                }
            }

			_hubproxy.ack_get_object_info_end(callbackid);
		}

		private hubmanager _hubmanager;
		private mongodbproxy _mongodbproxy;
	}
}

