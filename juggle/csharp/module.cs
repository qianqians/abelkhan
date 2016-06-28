using System;
using System.Reflection;
using System.Collections;

namespace service
{
    public class Imodule
    {
        public void process_event(ArrayList _event)
        {
            String func_name = (String)_event[1];

            Type type = GetType();
            MethodInfo method = type.GetMethod(func_name);

            if (_event.Count > 2)
            {
                method.Invoke(this, _event.GetRange(2, _event.Count - 2).ToArray());
            }
            else
            {
                method.Invoke(this, null);
            }
        }

		public String module_name;
    }
}
