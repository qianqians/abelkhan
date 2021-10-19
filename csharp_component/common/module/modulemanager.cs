using System;
using System.Collections;
using System.Collections.Generic;

namespace common
{
	public class modulemanager
	{
		public modulemanager()
		{
			modules = new Dictionary<string, imodule>();
		}

		public void add_module(String module_name, imodule _module)
		{
			modules.Add(module_name, _module);
		}

		public void process_module_mothed(String module_name, String func_name, IList<MsgPack.MessagePackObject> argvs)
		{
            if (modules.TryGetValue(module_name, out imodule _module))
			{
				_module.invoke(func_name, argvs);
			}
			else
            {
                log.log.err("do not have a module name:{0}", module_name);
				throw new moduleException(String.Format("modulemanager.process_module_mothed unreg module_name:%s!", module_name));
			}
		}

		private Dictionary<string, imodule> modules;
	}
}

