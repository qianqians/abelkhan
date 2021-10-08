using System;
using System.Collections;
using System.IO;

namespace hub
{
    public class hub_msg_handle
    {
        private hubmanager _hubmanager;
        private abelkhan.hub_call_hub_module _hub_call_hub_module;

        public hub_msg_handle(hubmanager _hubmanager_)
        {
            _hubmanager = _hubmanager_;

            _hub_call_hub_module = new abelkhan.hub_call_hub_module(abelkhan.modulemng_handle._modulemng);
            _hub_call_hub_module.on_reg_hub += reg_hub;
            _hub_call_hub_module.on_hub_call_hub_mothed += hub_call_hub_mothed;
        }

        public void reg_hub(string hub_name, string hub_type)
        {
            log.log.trace("hub:{0},{1} registered!", hub_name, hub_type);

            var rsp = (abelkhan.hub_call_hub_reg_hub_rsp)_hub_call_hub_module.rsp;
            _hubmanager.reg_hub(hub_name, hub_type, _hub_call_hub_module.current_ch);
            rsp.rsp();
        }

        public void hub_call_hub_mothed(byte[] rpc_argvs)
        {
            using (var st = new MemoryStream()) {
                st.Write(rpc_argvs);
                st.Position = 0;

                var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
                var _event = _serialization.Unpack(st);
                var module_name = (string)_event[0];
                var func_name = (string)_event[1];
                var _argvs = (ArrayList)_event[2];

                _hubmanager.current_hubproxy = _hubmanager.get_hub(_hub_call_hub_module.current_ch);
                hub._modules.process_module_mothed(module_name, func_name, _argvs);
                _hubmanager.current_hubproxy = null;
            }
        }

    }
}
