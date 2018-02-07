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
    }
}
