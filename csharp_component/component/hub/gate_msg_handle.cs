using abelkhan;
using MsgPack.Serialization;
using System;
using System.Collections;
using System.IO;

namespace hub
{
	public class gate_msg_handle
	{
        private readonly abelkhan.gate_call_hub_module _gate_call_hub_module;
        private readonly MessagePackSerializer<ArrayList> _serializer;

        public gate_msg_handle()
		{
            _serializer = MessagePackSerializer.Get<ArrayList>();

            _gate_call_hub_module = new abelkhan.gate_call_hub_module(abelkhan.modulemng_handle._modulemng);
            _gate_call_hub_module.on_client_disconnect += client_disconnect;
            _gate_call_hub_module.on_client_exception += client_exception;
            _gate_call_hub_module.on_client_call_hub += client_call_hub;
        }

        public void client_disconnect(String client_uuid)
        {
            Hub._gates.client_disconnect(client_uuid);
        }

        public void client_exception(String client_uuid)
        {
            Hub._gates.client_exception(client_uuid);
        }

        public Action<string> on_client_msg;
        public void client_call_hub(String uuid, byte[] rpc_argv)
		{
            try
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                st.Write(rpc_argv);
                st.Position = 0;
                var _event = _serializer.Unpack(st);

                var func = ((MsgPack.MessagePackObject)_event[0]).AsString();
                var argvs = ((MsgPack.MessagePackObject)_event[1]).AsList();

                Hub._gates.current_client_uuid = uuid;
                Hub._gates.client_connect(uuid, _gate_call_hub_module.current_ch.Value);
                Hub._modules.process_module_mothed(func, argvs);
                on_client_msg?.Invoke(uuid);
                Hub._gates.current_client_uuid = "";
            }
            catch (System.Exception e)
            {
                log.Log.err("call_hub exception:{0}", e);
                Hub._gates.client_exception(uuid);
            }
        }
	}
}

