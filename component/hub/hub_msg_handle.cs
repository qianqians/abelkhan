using System;
using System.Collections;

namespace hub
{
    public class hub_msg_handle
    {
        public hub_msg_handle(common.modulemanager _modulemanager_, hubmanager _hubmanager_)
        {
            _modulemanager = _modulemanager_;
            _hubmanager = _hubmanager_;
        }

        public void reg_hub(string hub_name)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hub:{0} registered!", hub_name);

            var _proxy = _hubmanager.reg_hub(hub_name, juggle.Imodule.current_ch);
            _proxy.reg_hub_sucess();
        }

        public void reg_hub_sucess()
        {
        }

        public void hub_call_hub_mothed(string module_name, string func_name, ArrayList argvs)
        {
            _modulemanager.process_module_mothed(module_name, func_name, argvs);
        }

        private common.modulemanager _modulemanager;
        private hubmanager _hubmanager;

    }
}
