using System;
using System.Collections;
using System.Collections.Generic;

namespace robot
{
    public class client_proxy
    {
        public client_proxy(juggle.Ichannel ch)
        {
            uuid = System.Guid.NewGuid().ToString();
            _client_call_gate = new caller.client_call_gate(ch);
        }

        public void heartbeats(Int64 tick)
        {
            _client_call_gate.heartbeats(tick);

            robot.timer.addticktime(tick + 30 * 1000, heartbeats);
        }

        public void connect_server(Int64 tick)
        {
            _client_call_gate.connect_server(uuid, tick);
        }

        public void cancle_server()
        {
            _client_call_gate.cancle_server();
        }

        public void get_logic()
        {
            _client_call_gate.get_logic();
        }

        public void connect_logic(string logic_uuid)
        {
            _client_call_gate.connect_logic(uuid, logic_uuid);
        }

        public void disconnect_logic(string logic_uuid)
        {
            _client_call_gate.disconnect_logic(uuid, logic_uuid);
        }

        public void connect_hub(string hub_name)
        {
            _client_call_gate.connect_hub(uuid, hub_name);
        }

        public void disconnect_hub(string hub_name)
        {
            _client_call_gate.disconnect_hub(uuid, hub_name);
        }

        public void call_logic(String logic_uuid, String module_name, String func_name, params object[] _argvs)
        {
            ArrayList _argvs_list = new ArrayList();
            foreach (var o in _argvs)
            {
                _argvs_list.Add(o);
            }

            _client_call_gate.forward_client_call_logic(logic_uuid, module_name, func_name, _argvs_list);
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

        public String uuid;
        public caller.client_call_gate _client_call_gate;
    }

    public class robot
    {
        public robot(Int64 robot_num)
        {
            timer = new service.timerservice();
            modulemanager = new common.modulemanager();

            _process = new juggle.process();
            _gate_call_client = new module.gate_call_client();
            _gate_call_client.onconnect_gate_sucess += on_ack_connect_gate;
            _gate_call_client.onack_get_logic += on_ack_get_logic;
            _gate_call_client.onconnect_logic_sucess += on_ack_connect_logic;
            _gate_call_client.onconnect_hub_sucess += on_ack_connect_hub;
            _gate_call_client.oncall_client += on_call_client;
            _process.reg_module(_gate_call_client);

            _conn = new service.connectnetworkservice(_process);

            _juggleservice = new service.juggleservice();
            _juggleservice.add_process(_process);

            proxys = new Dictionary<juggle.Ichannel, client_proxy>();

            _max_robot_num = robot_num;
            _robot_num = 0;
        }

        public delegate void onConnectGateHandle();
        public event onConnectGateHandle onConnectGate;
        private void on_ack_connect_gate()
        {
            timer.addticktime(timer.Tick + 30 * 1000, proxys[juggle.Imodule.current_ch].heartbeats);

            if ( (++_robot_num) < _max_robot_num )
            {
                var ch = _conn.connect(_ip, _port);
                var proxy = new client_proxy(ch);
                proxys.Add(ch, proxy);
                proxy.connect_server(timer.Tick);
            }
            else
            {
                Console.WriteLine("all robots connected");
            }

            if (onConnectGate != null)
            {
                onConnectGate();
            }
        }

        public delegate void onConnectLogicHandle(string _logic_uuid);
        public event onConnectLogicHandle onConnectLogic;
        private void on_ack_connect_logic(string _logic_uuid)
        {
            if (onConnectLogic != null)
            {
                onConnectLogic(_logic_uuid);
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

        public delegate void onAckGetLogicHandle(string _logic_uuid);
        public event onAckGetLogicHandle onAckGetLogic;
        private void on_ack_get_logic(string _logic_uuid)
        {
            if (onAckGetLogic != null)
            {
                onAckGetLogic(_logic_uuid);
            }
        }

        private void on_call_client(String module_name, String func_name, ArrayList argvs)
        {
            modulemanager.process_module_mothed(module_name, func_name, argvs);
        }

        public bool connect_server(String ip, short port, Int64 tick)
        {
            _ip = ip;
            _port = port;

            try
            {
                //for (int i = 0; i > _robot_num; i++)
                //{
                    var ch = _conn.connect(_ip, _port);
                    var proxy = new client_proxy(ch);
                    proxys.Add(ch, proxy);
                    proxy.connect_server(tick);
                //}
                
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        
        public void poll(Int64 tick)
        {
            timer.poll(tick);
            _juggleservice.poll(tick);
        }

        public client_proxy get_client_proxy(juggle.Ichannel ch)
        {
            if (proxys.ContainsKey(ch))
            {
                return proxys[ch];
            }

            return null;
        }

        public static service.timerservice timer;
        public common.modulemanager modulemanager;

        private service.connectnetworkservice _conn;
        private juggle.process _process;
        private module.gate_call_client _gate_call_client;
        private service.juggleservice _juggleservice;

        private Dictionary<juggle.Ichannel, client_proxy> proxys;

        private string _ip;
        private short _port;
        private Int64 _robot_num;
        private Int64 _max_robot_num;
    }
}
