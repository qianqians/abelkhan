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

            String func_name = ((MsgPack.MessagePackObject)_event[1]).AsString();

            Type type = GetType();
            MethodInfo method = type.GetMethod(func_name);

            if (_event.Count > 2)
            {
				object[] argv = new object[1];
				argv[0] = ((MsgPack.MessagePackObject)_event[2]).AsList();

				method.Invoke(this, argv);
            }
            else
            {
                method.Invoke(this, null);
            }

			current_ch = null;
        }

		public Ichannel current_ch;
		public String module_name;
    }
}
