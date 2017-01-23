using System;
using System.Collections;
using System.Threading;

namespace client
{
	public class client
	{
		public client()
		{
			uuid = System.Guid.NewGuid().ToString();
			timer = new service.timerservice();
			modulemanager = new common.modulemanager();

			_process = new juggle.process();
			_gate_call_client = new module.gate_call_client();
			_gate_call_client.onack_connect_server += on_ack_connect_server;
			_gate_call_client.oncall_client += on_call_client;
			_process.reg_module(_gate_call_client);

			_conn = new service.connectnetworkservice(_process);

			_juggleservice = new service.juggleservice();
			_juggleservice.add_process(_process);
        }

        private void heartbeats(Int64 tick)
        {
            _client_call_gate.heartbeats(tick);

            timer.addticktime(tick + 30 * 1000, heartbeats);
        }

        public delegate void onConnectServerHandle(String result);
		public event onConnectServerHandle onConnectServer;
		private void on_ack_connect_server(String result)
		{
			if (onConnectServer != null)
			{
				onConnectServer(result);
            }

            timer.addticktime(timer.Tick + 30*1000, heartbeats);
        }

		private void on_call_client(String module_name, String func_name, ArrayList argvs)
		{
			modulemanager.process_module_mothed(module_name, func_name, argvs);
		}

		public bool connect_server(String ip, short port, Int64 tick)
		{
			try
			{
				var ch = _conn.connect(ip, port);
				_client_call_gate = new caller.client_call_gate(ch);
				_client_call_gate.connect_server(uuid, tick);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public void cancle_server()
		{
			_client_call_gate.cancle_server();
		}

		public void call_logic(String module_name, String func_name, params object[] _argvs)
		{
			ArrayList _argvs_list = new ArrayList();
			foreach (var o in _argvs)
			{
				_argvs_list.Add(o);
			}
			
			_client_call_gate.forward_client_call_logic(module_name, func_name, _argvs_list);
		}

		public void poll(Int64 tick)
		{
			_juggleservice.poll(tick);
			timer.poll(tick);
        }

        private static void Main()
        {
            client _client = new client();

            Int64 tick = Environment.TickCount;
            _client.connect_server("139.129.96.47", 3236, tick);

            Int64 tickcount = 0;
            while (true)
            {
                Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
                if (tmptick < tick)
                {
                    tickcount += 1;
                    tmptick = tmptick + tickcount * UInt32.MaxValue;
                }
                tick = tmptick;

                _client.poll(tick);

                tmptick = (Environment.TickCount & UInt32.MaxValue);
                if (tmptick < tick)
                {
                    tickcount += 1;
                    tmptick = tmptick + tickcount * UInt32.MaxValue;
                }
                Int64 ticktime = (tmptick - tick);
                tick = tmptick;

                if (ticktime < 50)
                {
                    Thread.Sleep(15);
                }
            }
        }

        public String uuid;
		public service.timerservice timer;
		public common.modulemanager modulemanager;

		private service.connectnetworkservice _conn;
		private juggle.process _process;
		private module.gate_call_client _gate_call_client;
		private service.juggleservice _juggleservice;

		private caller.client_call_gate _client_call_gate;

	}
}

