using System;
using System.Collections.Generic;

namespace service
{
    class Icaller
    {
        public Icaller(Ichannel _ch)
        {
            ch = _ch;
        }

        public void call_module_method(String methodname, ArrayList argvs)
        {
            ArrayList _event;
            _event.Add(module_name);
            _event.Add(methodname);
            _event.Add(argvs);

            ch->push(_event);
        }

        private String module_name;
        private Ichannel ch;
    }
}
