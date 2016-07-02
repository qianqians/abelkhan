using System;
using System.Collections;
using System.Collections.Generic;

namespace service
{
    public class process
    {
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

                    String module_name = (String)_event[0];

                    Imodule module = (Imodule)module_set[module_name];
                    module.process_event(_event);
                }
            }
        }

        private List<Ichannel> event_set;
        private Hashtable module_set;
    }
}
