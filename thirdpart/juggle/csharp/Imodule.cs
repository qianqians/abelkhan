using System;
using System.Reflection;
using System.Collections;

namespace juggle
{
    public class Imodule
    {
		public void process_event(Ichannel _ch, ArrayList _event)
		{
			current_ch = _ch;

			String func_name = (String)_event[1];

			Type type = GetType();
			MethodInfo method = type.GetMethod(func_name);

			if (method != null)
			{
				if (_event.Count > 2)
				{
					object[] argv = new object[1];
					argv[0] = (ArrayList)_event[2];

					method.Invoke(this, argv);
				}
				else
				{
					method.Invoke(this, null);
				}

				current_ch = null;
			}
			else
			{
				Console.WriteLine("do not have a function named:" + func_name);
			}
		}

		public static Ichannel current_ch;
		public String module_name;
    }
}
