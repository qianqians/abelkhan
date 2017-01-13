using System;
using System.Collections;

namespace dbproxy
{
	public class logic_msg_handle
	{
		public logic_msg_handle(logicmanager _logicmanager_, mongodbproxy _mongodbproxy_, closehandle _closehandle_)
		{
			_logicmanager = _logicmanager_;
			_mongodbproxy = _mongodbproxy_;
			_closehandle = _closehandle_;
		}

		public void reg_logic(string uuid)
		{
			System.Console.WriteLine ("logic " + uuid + " connected");

			_closehandle.reg_logic ();
			var _logicproxy = _logicmanager.reg_logic(uuid, juggle.Imodule.current_ch);
			_logicproxy.reg_logic_sucess ();
		}

		public void create_persisted_object(Hashtable object_info, string callbackid)
		{
			_mongodbproxy.save(object_info);
			logicproxy _logicproxy = _logicmanager.get_logic (juggle.Imodule.current_ch);

			if (_logicproxy != null) 
			{
				_logicproxy.ack_create_persisted_object (callbackid);
			}
		}

		public void updata_persisted_object(Hashtable query_json, Hashtable object_info, string callbackid)
		{
			_mongodbproxy.update(query_json, object_info);

			logicproxy _logicproxy = _logicmanager.get_logic (juggle.Imodule.current_ch);

			if (_logicproxy != null) 
			{
				_logicproxy.ack_updata_persisted_object (callbackid);
			}
		}

		public void get_object_info(Hashtable query_json, string callbackid)
		{
			ArrayList _list = _mongodbproxy.find(0, 0, 0, query_json, null);

			logicproxy _logicproxy = _logicmanager.get_logic(juggle.Imodule.current_ch);

			if (_logicproxy == null) 
			{
				return;
			}

			int count = 0;
			ArrayList _datalist = new ArrayList ();
			foreach (var data in _list)
			{
				_datalist.Add (data);

				count++;

				if (count >= 100) 
				{
					_logicproxy.ack_get_object_info(callbackid, _datalist);

					count = 0;
					_datalist = new ArrayList ();
				}
			}
			if (count > 0 && count < 100) {
				_logicproxy.ack_get_object_info(callbackid, _datalist);
			}

			_logicproxy.ack_get_object_info_end(callbackid);
		}

		public void logic_closed()
		{
			_closehandle.logic_closed();
		}

		private logicmanager _logicmanager;
		private mongodbproxy _mongodbproxy;
		private closehandle _closehandle;
	}
}

