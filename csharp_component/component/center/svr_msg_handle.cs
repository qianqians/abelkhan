/*
 * svr_msg_handle
 * 2020/6/2
 * qianqians
 */

using System.Collections.Generic;

namespace abelkhan
{
    public class svr_msg_handle
    {
        private center_module _center_module;
        private svrmanager _svrmng;
        private closehandle _closehandle;

        private Dictionary<string, uint> name_serial;

        public svr_msg_handle(svrmanager svrs, closehandle closehandle)
        {
            _svrmng = svrs;
            _closehandle = closehandle;

            name_serial = new Dictionary<string, uint>();

            _center_module = new center_module(abelkhan.modulemng_handle._modulemng);
            _center_module.on_reg_server += reg_server;
            _center_module.on_heartbeat += heartbeat;
            _center_module.on_closed += closed;
        }

        private void reg_server(string type, string sub_type, string svr_name, string host, ushort port)
        {
            var name_prefix = sub_type;
            if (string.IsNullOrEmpty(name_prefix))
            {
                name_prefix = type;
            }
            if (string.IsNullOrEmpty(name_prefix))
            {
                log.log.err("reg server no type and hub_type!");
            }
            if (!name_serial.TryGetValue(name_prefix, out uint serial))
            {
                serial = 0;
                name_serial.Add(name_prefix, serial);
            }
            else
            {
                name_serial[name_prefix] = ++serial;
            }
            var name = svr_name;
            if (type == "hub" || type == "gate")
            {
                name = string.Format("{0}{1}", name_prefix, serial);
            }

            var rsp = (abelkhan.center_reg_server_rsp)_center_module.rsp;
            rsp.rsp(serial, name);

            _svrmng.for_each_hub((hubproxy _proxy) =>{
                _proxy.distribute_server_address(type, sub_type, name, host, (ushort)port);
            });

            if (type == "hub")
            {
                var _hubproxy = _svrmng.reg_hub(_center_module.current_ch, type, name);

                _svrmng.for_each_svr((svrproxy _proxy) =>{
                    _hubproxy.distribute_server_address(_proxy.type, _proxy.sub_type, _proxy.name, _proxy.host, _proxy.port);
                });
            }

            _svrmng.reg_svr(_center_module.current_ch, type, sub_type, name, host, (ushort)port);
        }

        private void heartbeat()
        {
            var _svr_proxy = _svrmng.get_svr(_center_module.current_ch);
            if (_svr_proxy != null)
            {
                _svr_proxy.timetmp = service.timerservice.Tick;
            }
        }

        private void closed()
        {
            var _svr_proxy = _svrmng.get_svr(_center_module.current_ch);
            if (_svr_proxy != null)
            {
                _svr_proxy.is_closed = true;
                _svr_proxy.closed_svr();
            }

            var _hub_proxy = _svrmng.get_hub(_center_module.current_ch);
            if (_hub_proxy != null)
            {
                _hub_proxy.is_closed = true;
            }

            if (_closehandle.is_closing && _svrmng.check_all_hub_closed())
            {
                _svrmng.close_db();
                _closehandle.is_close = true;
            }
        }
    }
}