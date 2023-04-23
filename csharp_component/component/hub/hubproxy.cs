using System;
using System.Collections.Generic;
using System.Collections;
using MsgPack.Serialization;
using System.IO;
using abelkhan;

namespace hub
{
    public class Hubproxy
    {
        private readonly abelkhan.hub_call_hub_caller _hub_call_hub_caller;
        private readonly MessagePackSerializer<ArrayList> _serializer = MessagePackSerializer.Get<ArrayList>();

        public Hubproxy(string hub_name, string hub_type, abelkhan.Ichannel ch)
        {
            name = hub_name;
            type = hub_type;
            _ch = ch;
            _hub_call_hub_caller = new abelkhan.hub_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void caller_hub(string func_name, ArrayList argvs)
        {
            using var st = MemoryStreamPool.mstMgr.GetStream();
            var _event = new ArrayList
            {
                func_name,
                argvs
            };

            _serializer.Pack(st, _event);
            st.Position = 0;

            _hub_call_hub_caller.hub_call_hub_mothed(st.ToArray());
        }

        public void client_seep(string client_uuid)
        {
            _hub_call_hub_caller.seep_client_gate(client_uuid, Hub._gates.get_client_gate_name(client_uuid));
        }

        public readonly string name;
        public readonly string type;
        public readonly abelkhan.Ichannel _ch;
    }
}
