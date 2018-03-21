/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class gm_center : juggle.Icaller 
    {
        public gm_center(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "gm_center";
        }

        public void confirm_gm(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("confirm_gm", _argv);
        }

        public void close_clutter(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("close_clutter", _argv);
        }

        public void reload()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("reload", _argv);
        }

    }
}
