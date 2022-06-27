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

        public void caller_hub(string func_name, ArrayList argvs)
        {
            var _serializer = MessagePackSerializer.Get<ArrayList>();
            using (var st = new MemoryStream())
            {
                var _event = new ArrayList();
                _event.Add(func_name);
                _event.Add(argvs);

                _serializer.Pack(st, _event);
                st.Position = 0;

                _hub_call_hub_caller.hub_call_hub_mothed(st.ToArray());
            }
        }

        public void client_seep(string client_uuid, string gate_name)
        {
            _hub_call_hub_caller.seep_client_gate(client_uuid, gate_name);
        }

        public string name;
        public string type;
        public abelkhan.Ichannel _ch;
    }
}
