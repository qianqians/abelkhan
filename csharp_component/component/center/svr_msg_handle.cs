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

        public svr_msg_handle(svrmanager svrs, closehandle closehandle)
        {
            _svrmng = svrs;
            _closehandle = closehandle;

            _center_module = new center_module(abelkhan.modulemng_handle._modulemng);
            _center_module.on_reg_server += reg_server;
            _center_module.on_reconn_reg_server += on_reconn_reg_server;
            _center_module.on_heartbeat += heartbeat;
            _center_module.on_closed += closed;
        }

        private void reg_server(string type, string svr_name, string host, ushort port)
        {
            var rsp = (abelkhan.center_reg_server_rsp)_center_module.rsp;
            rsp.rsp();

            _svrmng.for_each_hub((hubproxy _proxy) =>{
                _proxy.distribute_server_address(type, svr_name, host, port);
            });

            if (type == "hub")
            {
                var _hubproxy = _svrmng.reg_hub(_center_module.current_ch, type, svr_name);

                _svrmng.for_each_svr((svrproxy _proxy) =>{
                    _hubproxy.distribute_server_address(_proxy.type, _proxy.name, _proxy.host, _proxy.port);
                });
            }

            _svrmng.reg_svr(_center_module.current_ch, type, svr_name, host, port);
        }

        private void on_reconn_reg_server(string type, string svr_name, string host, ushort port)
        {
            var rsp = (abelkhan.center_reconn_reg_server_rsp)_center_module.rsp;
            rsp.rsp();

            _svrmng.for_each_new_hub((hubproxy _proxy) => {
                _proxy.distribute_server_address(type, svr_name, host, port);
            });

            if (type == "hub")
            {
                var _hubproxy = _svrmng.reg_hub(_center_module.current_ch, type, svr_name, true);

                _svrmng.for_each_new_svr((svrproxy _proxy) => {
                    _hubproxy.distribute_server_address(_proxy.type, _proxy.name, _proxy.host, _proxy.port);
                });
            }
            _svrmng.reg_svr(_center_module.current_ch, type, svr_name, host, port, true);
        }

        private void heartbeat(uint tick)
        {
            var rsp = (abelkhan.center_heartbeat_rsp)_center_module.rsp;
            rsp.rsp();

            var _svr_proxy = _svrmng.get_svr(_center_module.current_ch);
            if (_svr_proxy != null)
            {
                _svr_proxy.timetmp = service.timerservice.Tick;
                _svr_proxy.tick = tick;
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