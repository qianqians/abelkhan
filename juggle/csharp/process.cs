using System;
using System.Collections;
using System.Collections.Generic;

namespace juggle
{
    public class process
    {
		public process()
		{
			event_set = new List<Ichannel>();
			module_set = new Hashtable();
		}

        public void reg_channel(Ichannel ch)
        {
            event_set.Add(ch);
        }

        public void unreg_channel(Ichannel ch)
        {
            event_set.Remove(ch);
        }

		public void reg_module(Imodule module)
        {
			module_set.Add(module.module_name, module);
        }

		public void unreg_module(Imodule module)
        {
			module_set.Remove(module.module_name);
        }

        public void poll()
        {
            foreach (Ichannel ch in event_set)
            {
				while (true)
				{
					ArrayList _event = ch.pop();

					if (_event == null)
					{
						break;
					}

					String module_name = ((MsgPack.MessagePackObject)_event[0]).AsString();

					if (module_set.ContainsKey(module_name))
					{
						Imodule module = (Imodule)module_set[module_name];
						module.process_event(ch, _event);
					}
					else
					{
						Console.WriteLine("do not have a module named:" + module_name);
					}
				}
            }
        }

        private List<Ichannel> event_set;
        private Hashtable module_set;
    }
}
