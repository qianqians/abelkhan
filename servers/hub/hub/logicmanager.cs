using System;
using System.Collections;

namespace hub
{
	public class logicmanager
	{
		public logicmanager()
		{
			logicproxys = new Hashtable();
		}

		public void reg_logic(String uuid, juggle.Ichannel ch)
		{
			logicproxys.Add(uuid, new logicproxy(ch));
		}

		private bool has_logic(String uuid)
		{
			return logicproxys.ContainsKey(uuid);
		}

		private logicproxy get_logic(String uuid)
		{
			return (logicproxy)logicproxys[uuid];
		}

		public void call_logic_mothed(String logic_uuid, String module_name, String mothed_name, ArrayList argvs)
		{
			if (has_logic(logic_uuid))
			{
				var _logicproxy = get_logic(logic_uuid);

				_logicproxy.call_logic(module_name, mothed_name, argvs);
			}
		}

		public void call_group_logic(String[] logic_uuids, String module_name, String mothed_name, ArrayList argvs)
		{
			foreach (var logic_uuid in logic_uuids)
			{
				call_logic_mothed(logic_uuid, module_name, mothed_name, argvs);
			}
		}

		public void call_global_logic(String[] logic_uuids, String module_name, String mothed_name, ArrayList argvs)
		{
			foreach (var _logicproxy in logicproxys.Values)
			{
				((logicproxy)_logicproxy).call_logic(module_name, mothed_name, argvs);
			}
		}

		private Hashtable logicproxys;
	}
}

