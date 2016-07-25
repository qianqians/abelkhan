using System;
using System.Collections;
using System.Collections.Generic;

namespace logic
{
	public class logicmanager
	{
		public logicmanager(service.connectnetworkservice _conn)
		{
			_logic_conn = _conn;

			callLogicCallback_argv_set = new Dictionary<string, ArrayList>();
			connect_logicproxy = new Dictionary<juggle.Ichannel, logicproxy>();
			logics = new Dictionary<string, Tuple<string, short>>();
			lgoicproxys = new Dictionary<string, logicproxy>();
		}

		public void reg_logic(String uuid, String ip, short port)
		{
			logics.Add(uuid, new Tuple<string, short>(ip, port));
		}

		public logicproxy on_reg_logic(String uuid, juggle.Ichannel ch)
		{
			if (!lgoicproxys.ContainsKey(uuid))
			{
				var _proxy = new logicproxy(ch);
				lgoicproxys.Add(uuid, _proxy);

				return _proxy;
			}

			return lgoicproxys[uuid];
		}

		public void on_ack_reg_logic(String uuid, juggle.Ichannel ch)
		{
			if (!lgoicproxys.ContainsKey(uuid))
			{
				if (connect_logicproxy.ContainsKey(ch))
				{
					var _proxy = connect_logicproxy[ch];
					lgoicproxys.Add(uuid, _proxy);
				}
			}
		}

		public void call_logic_mothed(String svr_uuid, String module_name, String func_name, params object[] _argvs)
		{
			ArrayList _argvs_list = new ArrayList();
			foreach (var o in _argvs)
			{
				_argvs_list.Add(o);
			}
			
			if (lgoicproxys.ContainsKey(svr_uuid))
			{
				lgoicproxys[svr_uuid].call_logic(module_name, func_name, _argvs_list);
			}
			else
			{
				if (logics.ContainsKey(svr_uuid))
				{
					var ip = logics[svr_uuid].Item1;
					var port = logics[svr_uuid].Item2;

					var ch = _logic_conn.connect(ip, port);

					var _proxy = new logicproxy(ch);
					var callbackid = System.Guid.NewGuid().ToString();
					_proxy.reg_logic(callbackid);

					var _argvs_ = new ArrayList();
					_argvs_.Add(svr_uuid);
					_argvs_.Add(module_name);
					_argvs_.Add(func_name);
					_argvs_.Add(_argvs_list);
					callLogicCallback_argv_set.Add(callbackid, _argvs_);
				}
			}
		}

		public void do_callback(String callbackid)
		{
			if (callLogicCallback_argv_set.ContainsKey(callbackid))
			{
				var _argvs = callLogicCallback_argv_set[callbackid];
				var svr_uuid = (String)_argvs[0];
				var module_name = (String)_argvs[1];
				var func_name = (String)_argvs[2];
				var argvs = (ArrayList)_argvs[3];

				if (lgoicproxys.ContainsKey(svr_uuid))
				{
					lgoicproxys[svr_uuid].call_logic(module_name, func_name, argvs);
				}

				callLogicCallback_argv_set.Remove(callbackid);
			}
		}

		private Dictionary<String, ArrayList> callLogicCallback_argv_set;

		private Dictionary<juggle.Ichannel, logicproxy> connect_logicproxy;
		private Dictionary<String, Tuple<String, short> > logics;

		private Dictionary<String, logicproxy> lgoicproxys;

		private service.connectnetworkservice _logic_conn;

	}
}

