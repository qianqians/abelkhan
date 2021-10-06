using System;
using System.Collections.Generic;
using System.Collections;
using MsgPack.Serialization;
using System.IO;

namespace hub
{
    public class hubproxy
    {
        private abelkhan.hub_call_hub_caller _hub_call_hub_caller;

        public hubproxy(string hub_name, string hub_type, abelkhan.Ichannel ch)
        {
            name = hub_name;
            type = hub_type;
            _ch = ch;
            _hub_call_hub_caller = new abelkhan.hub_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void caller_hub(string module_name, string func_name, ArrayList argvs)
        {
            var _serializer = MessagePackSerializer.Get<ArrayList>();
            using (var st = new MemoryStream())
            {
                _serializer.Pack(st, argvs);
                _hub_call_hub_caller.hub_call_hub_mothed(module_name, func_name, st.ToArray());
            }
        }

        public string name;
        public string type;
        public abelkhan.Ichannel _ch;
    }
}
