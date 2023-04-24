using Abelkhan;
using System;
using System.Collections;
using System.IO;

namespace Hub
{
    public class hub_msg_handle
    {
        private readonly HubManager _hubmanager;
        private readonly GateManager _gatemanager;
        private readonly Abelkhan.hub_call_hub_module _hub_call_hub_module;
        private readonly MsgPack.Serialization.MessagePackSerializer<ArrayList> _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();

        public hub_msg_handle(HubManager _hubmanager_, GateManager _gatemanager_)
        {
            _hubmanager = _hubmanager_;
            _gatemanager = _gatemanager_;

            _hub_call_hub_module = new Abelkhan.hub_call_hub_module(Abelkhan.ModuleMgrHandle._modulemng);
            _hub_call_hub_module.on_reg_hub += reg_hub;
            _hub_call_hub_module.on_seep_client_gate += seep_client_gate;
            _hub_call_hub_module.on_hub_call_hub_mothed += hub_call_hub_mothed;
        }

        public void reg_hub(string hub_name, string hub_type)
        {
            Log.Log.trace("hub:{0},{1} registered!", hub_name, hub_type);

            var rsp = (Abelkhan.hub_call_hub_reg_hub_rsp)_hub_call_hub_module.rsp.Value;
            _hubmanager.reg_hub(hub_name, hub_type, _hub_call_hub_module.current_ch.Value);
            rsp.rsp();
        }

        private void seep_client_gate(string client_uuid, string gate_name)
        {
            Log.Log.trace("seep_client_gate client_uuid:{0}, gate_name:{1}!", client_uuid, gate_name);

            var rsp = (Abelkhan.hub_call_hub_seep_client_gate_rsp)_hub_call_hub_module.rsp.Value;
            var _proxy = _gatemanager.client_seep(client_uuid, gate_name);
            if (_proxy != null)
            {
                _proxy.reverse_reg_client_hub(client_uuid).callBack(() => {
                    rsp.rsp();
                }, (err) => {
                    rsp.err(err);
                }).timeout(1000, () => {
                    rsp.err(Abelkhan.framework_error.enum_framework_timeout);
                });
            }
            else
            {
                rsp.err(Abelkhan.framework_error.enum_framework_gate_exception);
            }
        }

        public void hub_call_hub_mothed(byte[] rpc_argvs)
        {
            try
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                st.Write(rpc_argvs);
                st.Position = 0;

                var _event = _serialization.Unpack(st);

                var func = ((MsgPack.MessagePackObject)_event[0]).AsString();
                var argvs = ((MsgPack.MessagePackObject)_event[1]).AsList();

                if (_hubmanager.get_hub(_hub_call_hub_module.current_ch.Value, out HubProxy _proxy))
                {
                    _hubmanager.current_hubproxy = _proxy;
                    Hub._modules.process_module_mothed(func, argvs);
                    _hubmanager.current_hubproxy = null;
                }
                else
                {
                    Log.Log.err("hub_call_hub_mothed not exist hubproxy!");
                }
            }
            catch(System.Exception ex)
            {
                Log.Log.err("hub_call_hub_mothed, ex:{0}", ex);
            }
        }

    }
}
