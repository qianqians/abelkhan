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
			_gate_call_client.onconnect_gate_sucess += on_ack_connect_gate;
            _gate_call_client.onconnect_hub_sucess += on_ack_connect_hub;
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

        public delegate void onConnectGateHandle();
		public event onConnectGateHandle onConnectGate;
		private void on_ack_connect_gate()
		{
            timer.addticktime(service.timerservice.Tick + 30 * 1000, heartbeats);

            if (onConnectGate != null)
			{
                onConnectGate();
            }
        }

        public delegate void onConnectHubHandle(string _hub_name);
        public event onConnectHubHandle onConnectHub;
        private void on_ack_connect_hub(string _hub_name)
        {
            if (onConnectHub != null)
            {
                onConnectHub(_hub_name);
            }
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

        public void connect_hub(string hub_name)
        {
            _client_call_gate.connect_hub(uuid, hub_name);
        }

        public void disconnect_hub(string hub_name)
        {
            _client_call_gate.disconnect_hub(uuid, hub_name);
        }

        public void call_hub(String hub_name, String module_name, String func_name, params object[] _argvs)
        {
            ArrayList _argvs_list = new ArrayList();
            foreach (var o in _argvs)
            {
                _argvs_list.Add(o);
            }

            _client_call_gate.forward_client_call_hub(hub_name, module_name, func_name, _argvs_list);
        }

        public Int64 poll()
        {
            Int64 tick = timer.poll();
            _juggleservice.poll(tick);

            return tick;
        }

        private static void Main()
        {
            client _client = new client();

            Int64 old_tick = 0;
            Int64 tick = 0;
            while (true)
            {
                old_tick = tick;
                tick = _client.poll();
                
                Int64 ticktime = (tick - old_tick);
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

