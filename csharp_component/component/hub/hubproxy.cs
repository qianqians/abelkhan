using System;
using System.Collections.Generic;
using System.Collections;
using MsgPack.Serialization;
using System.IO;
using Abelkhan;

namespace Hub
{
    public class HubProxy
    {
        private readonly Abelkhan.hub_call_hub_caller _hub_call_hub_caller;
        private readonly MessagePackSerializer<ArrayList> _serializer = MessagePackSerializer.Get<ArrayList>();

        public HubProxy(string hub_name, string hub_type, Abelkhan.Ichannel ch)
        {
            name = hub_name;
            type = hub_type;
            _ch = ch;
            _hub_call_hub_caller = new Abelkhan.hub_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
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
        public readonly Abelkhan.Ichannel _ch;
    }
}
