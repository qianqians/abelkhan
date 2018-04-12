using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace juggle
{
    public class Icaller
    {
        public Icaller(Ichannel _ch)
        {
            ch = _ch;
        }

        public void call_module_method(String methodname, ArrayList argvs)
        {
			ArrayList _event = new ArrayList();
            _event.Add(module_name);
            _event.Add(methodname);
            _event.Add(argvs);

            try
            {
                ch.push(_event);
            }
            catch (Json.Exception)
            {
                throw new juggle.Exception("error argvs");
            }
        }

        protected String module_name;
        private Ichannel ch;
    }
}
