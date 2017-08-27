using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace juggle
{
    public class process
    {
		public process()
		{
            add_event = new List<Ichannel>();
            remove_event = new List<Ichannel>();

            event_set = new List<Ichannel>();
			module_set = new Hashtable();
		}

        public void reg_channel(Ichannel ch)
        {
            Monitor.Enter(add_event);
            add_event.Add(ch);
            Monitor.Exit(add_event);
        }

        public void unreg_channel(Ichannel ch)
        {
            Monitor.Enter(remove_event);
            remove_event.Add(ch);
            Monitor.Exit(remove_event);
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
            Monitor.Enter(add_event);
            foreach(var ch in add_event)
            {
                event_set.Add(ch);
            }
            add_event.Clear();
            Monitor.Exit(add_event);

            Monitor.Enter(remove_event);
            foreach (var ch in remove_event)
            {
                event_set.Remove(ch);
            }
            remove_event.Clear();
            Monitor.Exit(remove_event);

            foreach (Ichannel ch in event_set)
            {
				while (true)
				{
                    ArrayList _event = null;
                    try
                    {
                        _event = ch.pop();

                    }
                    catch (Exception e)
                    {
                        throw new juggle.Exception(string.Format("channel pop, System.Exception:{0}", e));
                    }

                    try
                    {
                        if (_event == null)
                        {
                            break;
                        }

                        if (_event.Count < 2)
                        {
                            throw new juggle.Exception(string.Format("error msg, {0}", _event[0]));
                        }

                        String module_name = (String)_event[0];

                        if (module_set.ContainsKey(module_name))
                        {
                            Imodule _module = (Imodule)module_set[module_name];
                            _module.process_event(ch, _event);
                        }
                        else
                        {
                            throw new juggle.Exception(string.Format("do not have a module name:{0}, function name:{1}", module_name, (String)_event[1]));
                        }
                    }
                    catch (Exception e)
                    {
                        throw new juggle.Exception(string.Format("process event, System.Exception:{0}", e));
                    }
                }
            }
        }

        private List<Ichannel> add_event;
        private List<Ichannel> remove_event;

        private List<Ichannel> event_set;
        private Hashtable module_set;
    }
}
