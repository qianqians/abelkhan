using System;
using System.Collections;
using System.Collections.Generic;

namespace gm
{
    class center_proxy
    {
        public center_proxy(abelkhan.Ichannel ch)
        {
            center_caller = new abelkhan.gm_center_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void confirm_gm(string gm_name)
        {
            center_caller.confirm_gm(gm_name);
        }

        public void close_clutter(string gm_name)
        {
            center_caller.close_clutter(gm_name);
        }

        public void reload(string gm_name, string argv)
        {
            center_caller.reload(gm_name, argv);
        }

        private abelkhan.gm_center_caller center_caller;
    }

    class gm
    {
        public gm(string[] args, string _gm_name)
        {
            gm_name = _gm_name;

            abelkhan.config _config = new abelkhan.config(args[0]);
            if (args.Length > 1)
            {
                _config = _config.get_value_dict(args[1]);
            }

            var gm_ip = _config.get_value_string("gm_ip");
            var gm_port = (short)_config.get_value_int("gm_port");
            var _socket = abelkhan.connectservice.connect(System.Net.IPAddress.Parse(gm_ip), gm_port);
            ch = new abelkhan.rawchannel(_socket);
            _center_proxy = new center_proxy(ch);
            _center_proxy.confirm_gm(gm_name);

            timer = new service.timerservice();
        }

        public Int64 poll()
        {
            Int64 tick_begin = timer.poll();

            try
            {
                while (true)
                {
                    ArrayList _event = null;
                    lock (ch)
                    {
                        _event = ch.pop();
                    }
                    if (_event == null)
                    {
                        break;
                    }
                    abelkhan.modulemng_handle._modulemng.process_event(ch, _event);
                }
            }
            catch (abelkhan.Exception e)
            {
                log.log.err(e.Message);
            }
            catch (System.Exception e)
            {
                log.log.err("{0}", e);
            }

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
                log.log.err("non input start argv");
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
        private abelkhan.rawchannel ch;
        private service.timerservice timer;
        private center_proxy _center_proxy;
    }
}
