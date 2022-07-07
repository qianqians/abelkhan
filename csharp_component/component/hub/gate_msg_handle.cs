using MsgPack.Serialization;
using System;
using System.Collections;
using System.IO;

namespace hub
{
	public class gate_msg_handle
	{
        public abelkhan.gate_call_hub_module _gate_call_hub_module;

        public gate_msg_handle()
		{
            _gate_call_hub_module = new abelkhan.gate_call_hub_module(abelkhan.modulemng_handle._modulemng);
            _gate_call_hub_module.on_client_disconnect += client_disconnect;
            _gate_call_hub_module.on_client_exception += client_exception;
            _gate_call_hub_module.on_client_call_hub += client_call_hub;
        }

        public void client_disconnect(String client_uuid)
        {
            hub._gates.client_disconnect(client_uuid);
        }

        public void client_exception(String client_uuid)
        {
            hub._gates.client_exception(client_uuid);
        }

        public Action<string> on_client_msg;
        public void client_call_hub(String uuid, byte[] rpc_argv)
		{
            try
            {
                var _serializer = MessagePackSerializer.Get<ArrayList>();
                using (var st = new MemoryStream())
                {
                    st.Write(rpc_argv);
                    st.Position = 0;
                    var _event = _serializer.Unpack(st);

                    var func = ((MsgPack.MessagePackObject)_event[0]).AsString();
                    var argvs = ((MsgPack.MessagePackObject)_event[1]).AsList();

                    hub._gates.current_client_uuid = uuid;
                    hub._gates.client_connect(uuid, _gate_call_hub_module.current_ch.Value);
                    hub._modules.process_module_mothed(func, argvs);
                    on_client_msg?.Invoke(uuid);
                    hub._gates.current_client_uuid = "";
                }
            }
            catch (Exception e)
            {
                log.log.err("call_hub exception:{0}", e);
                hub._gates.client_exception(uuid);
            }
        }
	}
}

