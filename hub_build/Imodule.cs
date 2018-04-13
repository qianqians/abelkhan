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

            try
            {
                String func_name = (String)_event[1];

                Type type = GetType();
                MethodInfo method = type.GetMethod(func_name);

                if (method != null)
                {
                    try
                    {
                        object[] argv = new object[1];
                        if (_event.Count > 2)
                        {
                            argv[0] = (ArrayList)_event[2];
                        }
                        method.Invoke(this, argv);

                        current_ch = null;
                    }
                    catch (Exception e)
                    {
                        throw new juggle.Exception(string.Format("function name:{0} System.Exception:{1}", func_name, e));
                    }
                }
                else
                {
                    throw new juggle.Exception(string.Format("do not have a function named::{0}", func_name));
                }
            }
            catch (Exception e)
            {
                throw new juggle.Exception(string.Format("System.Exception:{0}", e));
            }
        }

		public static Ichannel current_ch;
		public String module_name;
    }
}
