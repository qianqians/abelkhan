using System;
using System.Collections;
using System.Collections.Generic;

namespace GM
{
    class CenterProxy
    {
        public CenterProxy(Abelkhan.Ichannel ch)
        {
            center_caller = new Abelkhan.gm_center_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
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

        private Abelkhan.gm_center_caller center_caller;
    }

    class GM
    {
        public GM(string[] args, string _gm_name)
        {
            gm_name = _gm_name;

            Abelkhan.Config _config = new Abelkhan.Config(args[0]);
            if (args.Length > 1)
            {
                _config = _config.get_value_dict(args[1]);
            }

            var gm_ip = _config.get_value_string("gm_ip");
            var gm_port = (short)_config.get_value_int("gm_port");
            var _socket = Abelkhan.ConnectService.connect(System.Net.IPAddress.Parse(gm_ip), gm_port);
            ch = new Abelkhan.RawChannel(_socket);
            _center_proxy = new CenterProxy(ch);
            _center_proxy.confirm_gm(gm_name);

            timer = new Service.Timerservice();
        }

        public long poll()
        {
            long tick_begin = timer.refresh(); 

            try
            {
                while (true)
                {
                    if (!Abelkhan.EventQueue.msgQue.TryDequeue(out Tuple<Abelkhan.Ichannel, ArrayList> _event))
                    {
                        break;
                    }
                    Abelkhan.ModuleMgrHandle._modulemng.process_event(_event.Item1, _event.Item2);
                }
            }
            catch (Abelkhan.Exception e)
            {
                Log.Log.err(e.Message);
            }
            catch (System.Exception e)
            {
                Log.Log.err("{0}", e);
            }

            timer.poll();

            long tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

        public void output_cmd()
        {
            Console.WriteLine("Enter gm cmd:");
            Console.WriteLine(" close-----close clutter");
            Console.WriteLine(" close_zone-----close zone");
            Console.WriteLine(" reload----reload hub");
        }

        public delegate void Handle(string[] cmds);
        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Log.Log.err("non input start argv");
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

            GM _gm = new GM(args, gm_name);
            bool runing = true;

            Dictionary<string, Handle> cmd_callback = new Dictionary<string, Handle>();
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
        private Abelkhan.Ichannel ch;
        private Service.Timerservice timer;
        private CenterProxy _center_proxy;
    }
}
