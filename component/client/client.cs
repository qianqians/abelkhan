using System;
using System.Collections;
using System.Threading;

namespace client
{
	public class client
	{
        public delegate void onDisConnectHandle();
        public event onDisConnectHandle onDisConnect;

        public client()
		{
			uuid = System.Guid.NewGuid().ToString();
			timer = new service.timerservice();
			modulemanager = new common.modulemanager();

			var _process = new juggle.process();
			_gate_call_client = new module.gate_call_client();
			_gate_call_client.onconnect_gate_sucess += on_ack_connect_gate;
            _gate_call_client.onconnect_hub_sucess += on_ack_connect_hub;
            _gate_call_client.onack_heartbeats += on_ack_heartbeats;
            _gate_call_client.oncall_client += on_call_client;
			_process.reg_module(_gate_call_client);
			_conn = new service.connectnetworkservice(_process);
            _conn.onChannelDisconnect += on_disconnect;

            var _udp_process = new juggle.process();
            _gate_call_client_fast = new module.gate_call_client_fast();
            _gate_call_client_fast.onconfirm_refresh_udp_end_point += onconfirm_refresh_udp_end_point;
            _gate_call_client_fast.oncall_client += on_call_client;
            _udp_process.reg_module(_gate_call_client_fast);
            _udp_conn = new service.udpconnectnetworkservice(_udp_process);

            _juggleservice = new service.juggleservice();
            _juggleservice.add_process(_process);
            _juggleservice.add_process(_udp_process);

            _heartbeats = 0;
            _is_enable_heartbeats = false;
            connect_state = false;
        }

        private void on_disconnect(juggle.Ichannel ch)
        {
            connect_state = false;

            if (onDisConnect != null)
            {
                onDisConnect();
            }
        }

        private void heartbeats(Int64 tick)
        {
            if (!connect_state)
            {
                return;
            }

            if (_is_enable_heartbeats && (_heartbeats < (tick - 20 * 1000)))
            {
                log.log.trace(new System.Diagnostics.StackFrame(), tick, "heartbeats:{0}", _heartbeats);

                if (onDisConnect != null)
                {
                    onDisConnect();
                }
            }
            else
            {
                _client_call_gate.heartbeats(tick);

                timer.addticktime(5 * 1000, heartbeats);
            }
        }

        private void on_ack_heartbeats()
        {
            _heartbeats = service.timerservice.Tick;
        }

        private void refresh_udp_link(Int64 tick)
        {
            if (!connect_state)
            {
                return;
            }

            _client_call_gate_fast.refresh_udp_end_point();

            timer.addticktime(10 * 1000, refresh_udp_link);
        }

        public delegate void onConnectGateHandle();
		public event onConnectGateHandle onConnectGate;
		private void on_ack_connect_gate()
		{
            udp_ch = _udp_conn.connect(_udp_ip, _udp_port);
            _client_call_gate_fast = new caller.client_call_gate_fast(udp_ch);
            _client_call_gate_fast.refresh_udp_end_point();

            _heartbeats = service.timerservice.Tick;
            _client_call_gate.heartbeats(service.timerservice.Tick);

            if (!is_reconnect)
            {
                timer.addticktime(5 * 1000, heartbeats);
                timer.addticktime(10 * 1000, refresh_udp_link);
            }

            if (onConnectGate != null)
			{
                onConnectGate();
            }

            connect_state = true;
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

        private void onconfirm_refresh_udp_end_point()
        {
            _client_call_gate_fast.confirm_create_udp_link(uuid);
        }

        private void on_call_client(String module_name, String func_name, ArrayList argvs)
		{
			modulemanager.process_module_mothed(module_name, func_name, argvs);
		}

        public bool reconnect_server(String tcp_ip, short tcp_port, String udp_ip, short udp_port, Int64 tick)
        {
            try
            {
                tcp_ch.disconnect();
                udp_ch.disconnect();

                uuid = System.Guid.NewGuid().ToString();
                is_reconnect = true;

                tcp_ch = _conn.connect(tcp_ip, tcp_port);
                _client_call_gate = new caller.client_call_gate(tcp_ch);
                _client_call_gate.connect_server(uuid, tick);

                _udp_ip = udp_ip;
                _udp_port = udp_port;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

		public bool connect_server(String tcp_ip, short tcp_port, String udp_ip, short udp_port, Int64 tick)
		{
			try
			{
                is_reconnect = false;

                tcp_ch = _conn.connect(tcp_ip, tcp_port);
				_client_call_gate = new caller.client_call_gate(tcp_ch);
				_client_call_gate.connect_server(uuid, tick);

                _udp_ip = udp_ip;
                _udp_port = udp_port;
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

        public void enable_heartbeats()
        {
            _client_call_gate.enable_heartbeats();

            _is_enable_heartbeats = true;
            _heartbeats = service.timerservice.Tick;
        }

        public void disable_heartbeats()
        {
            _client_call_gate.disable_heartbeats();

            _is_enable_heartbeats = false;
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

            System.GC.Collect();

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
                if (ticktime < 100)
                {
                    Thread.Sleep(15);
                }
            }
        }

        public String uuid;
		public service.timerservice timer;
		public common.modulemanager modulemanager;

        private Int64 _heartbeats;
        private bool _is_enable_heartbeats;

        private service.connectnetworkservice _conn;
        private juggle.Ichannel tcp_ch;
		private module.gate_call_client _gate_call_client;
		private caller.client_call_gate _client_call_gate;

        private bool connect_state;
        private bool is_reconnect;
        private string _udp_ip;
        private short _udp_port;
        private service.udpconnectnetworkservice _udp_conn;
        private juggle.Ichannel udp_ch;
        private module.gate_call_client_fast _gate_call_client_fast;
        private caller.client_call_gate_fast _client_call_gate_fast;

        private service.juggleservice _juggleservice;

    }
}

