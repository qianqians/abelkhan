/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class gm_center : juggle.Icaller 
    {
        public gm_center(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "gm_center";
        }

        public void close_clutter()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("close_clutter", _argv);
        }

    }
}
