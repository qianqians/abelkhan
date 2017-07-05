using System;
using common;

namespace client
{
    class login : imodule
    {
        public void login_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "login sucess");
        }

        public void udp_link_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "udp link sucess");
        }
    }
}
