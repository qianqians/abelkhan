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
        private Abelkhan.hub_call_hub_caller _hub_call_hub_caller;
        private readonly MessagePackSerializer<ArrayList> _serializer = MessagePackSerializer.Get<ArrayList> ();

        public HubProxy(string hub_name, string hub_type)
        {
            name = hub_name;
            type = hub_type;
        }

        public void set_redis_ch(Abelkhan.Ichannel ch)
        {
            _redis_ch = ch;
            if (_hub_call_hub_caller == null)
            {
                _hub_call_hub_caller = new Abelkhan.hub_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
            }
        }

        public void set_tcp_ch(Abelkhan.Ichannel ch)
        {
            _tcp_ch = ch;
            if (_hub_call_hub_caller == null)
            {
                _hub_call_hub_caller = new Abelkhan.hub_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
            }
            else
            {
                _hub_call_hub_caller.reset_ch(ch);
            }
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
        public Abelkhan.Ichannel _redis_ch;
        public Abelkhan.Ichannel _tcp_ch;
    }
}
