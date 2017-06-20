using System;
using System.Collections.Generic;

namespace gm
{
    class center_proxy
    {
        public center_proxy(juggle.Ichannel ch)
        {
            center_caller = new caller.gm_center(ch);
        }

        public void confirm_gm(string gm_name)
        {
            center_caller.confirm_gm(gm_name);
        }

        public void close_clutter(string gm_name)
        {
            center_caller.close_clutter(gm_name);
        }

        private caller.gm_center center_caller;
    }

    class gm
    {
        public gm(string[] args)
        {
            Console.WriteLine("Enter gm name");
            gm_name = Console.ReadLine();

            config.config _config = new config.config(args[0]);
            if (args.Length > 1)
            {
                _config = _config.get_value_dict(args[1]);
            }

            juggle.process _gm_process = new juggle.process();
            _conn_center = new service.connectnetworkservice(_gm_process);

            var gm_ip = _config.get_value_string("gm_ip");
            var gm_port = (short)_config.get_value_int("gm_port");
            _center_proxy= new center_proxy(_conn_center.connect(gm_ip, gm_port));
            _center_proxy.confirm_gm(gm_name);

            timer = new service.timerservice();

            _juggle_service = new service.juggleservice();
            _juggle_service.add_process(_gm_process);

            output_cmd();
        }

        public Int64 poll()
        {
            Int64 tick = timer.poll();

            try
            {
                _juggle_service.poll(tick);
            }
            catch (juggle.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick, e.Message);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick, "{0}", e);
            }
            
            _conn_center.poll(tick);

            return tick;
        }

        public void output_cmd()
        {
            Console.WriteLine("Enter gm cmd:");
            Console.WriteLine(" close----close clutter");
        }

        public delegate void handle();
        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "non input start argv");
                return;
            }

            gm _gm = new gm(args);
            bool runing = true;

            Dictionary<String, handle> cmd_callback = new Dictionary<string, handle>();
            cmd_callback.Add("close", () => { _gm._center_proxy.close_clutter(_gm.gm_name); runing = false; });

            Int64 old_tick = 0;
            Int64 tick = 0;
            while (runing)
            {
                old_tick = tick;
                tick = _gm.poll();

                string cmd = Console.ReadLine();
                if (cmd_callback.ContainsKey(cmd))
                {
                    cmd_callback[cmd]();
                }
                else
                {
                    Console.WriteLine("invalid gm cmd!");
                }

                _gm.output_cmd();

                Int64 tmp = tick - old_tick;
                if (tmp < 50)
                {
                    System.Threading.Thread.Sleep(15);
                }
            }
        }

        private string gm_name;

        private service.connectnetworkservice _conn_center;
        private service.juggleservice _juggle_service;
        private service.timerservice timer;

        private center_proxy _center_proxy;
    }
}
