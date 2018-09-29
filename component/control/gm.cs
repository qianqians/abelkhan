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

        public void close_zone(string gm_name, Int64 zone_id)
        {
            center_caller.close_zone(gm_name, zone_id);
        }

        public void reload(string gm_name, string argv)
        {
            center_caller.reload(gm_name, argv);
        }

        private caller.gm_center center_caller;
    }

    class gm
    {
        public gm(string[] args, string _gm_name)
        {
            gm_name = _gm_name;

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
        }

        public Int64 poll()
        {
            Int64 tick_begin = timer.poll();

            try
            {
                _juggle_service.poll(tick_begin);
            }
            catch (juggle.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, e.Message);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, "{0}", e);
            }

            System.GC.Collect();

            Int64 tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

        public void output_cmd()
        {
            Console.WriteLine("Enter gm cmd:");
            Console.WriteLine(" close-----close clutter");
            Console.WriteLine(" close_zone-----close zone");
            Console.WriteLine(" reload----reload hub");
        }

        public delegate void handle(string[] cmds);
        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "non input start argv");
                return;
            }

            string gm_name = null;
            string[] cmd = null;
            if (args.Length > 3)
            {
                gm_name = args[2];

                cmd = new string[2];
                for (int i = 3; i < args.Length; i++)
                {
                    cmd[i - 3] = args[i];
                }
            }
            else
            {
                Console.WriteLine("Enter gm name:");
                gm_name = Console.ReadLine();
            }

            gm _gm = new gm(args, gm_name);
            bool runing = true;

            Dictionary<String, handle> cmd_callback = new Dictionary<string, handle>();
            cmd_callback.Add("close", (string[] cmds) => {
                _gm._center_proxy.close_clutter(_gm.gm_name);
            });
            cmd_callback.Add("close_zone", (string[] cmds) => {
                _gm._center_proxy.close_zone(_gm.gm_name, Int64.Parse(cmds[1]));
            });
            cmd_callback.Add("reload", (string[] cmds) => {
                _gm._center_proxy.reload(_gm.gm_name, cmds[1]);
            });

            while (runing)
            {
                var tmp = _gm.poll();

                if (cmd != null)
                {
                    if (cmd_callback.ContainsKey(cmd[0]))
                    {
                        cmd_callback[cmd[0]](cmd);
                    }
                    else
                    {
                        Console.WriteLine("invalid gm cmd!");
                    }

                    System.Threading.Thread.Sleep(1500);
                    runing = false;
                }
                else
                {
                    _gm.output_cmd();

                    string cmd1 = Console.ReadLine();
                    string[] cmds = cmd1.Split(' ');
                    if (cmd_callback.ContainsKey(cmds[0]))
                    {
                        cmd_callback[cmds[0]](cmds);
                    }
                    else
                    {
                        Console.WriteLine("invalid gm cmd!");
                    }
                }

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
