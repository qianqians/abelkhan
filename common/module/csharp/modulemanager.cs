using System;
using System.Collections;

namespace common
{
	public class modulemanager
	{
		public modulemanager()
		{
			modules = new Hashtable();
		}

		public void add_module(String module_name, imodule _module)
		{
			modules.Add(module_name, _module);
		}

		public void process_module_mothed(String module_name, String func_name, ArrayList argvs)
		{
            if (modules.ContainsKey(module_name))
			{
				imodule _module = (imodule)modules[module_name];

				var _type = _module.GetType();
				var method = _type.GetMethod(func_name);
				if (method != null)
				{
					try
					{
						if (argvs.Count > 0)
						{
							method.Invoke(_module, argvs.ToArray());
						}
						else
						{
							method.Invoke(_module, null);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("call rpc error {0}", e);
					}
				}
                else
                {
                    Console.WriteLine("do not have a func named " + func_name);
                }
			}
			else
			{
				Console.WriteLine("do not have a module named " + module_name);
			}
		}

		private Hashtable modules;
	}
}

