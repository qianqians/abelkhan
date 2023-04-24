/*
 * svr_msg_handle
 * 2020/6/2
 * qianqians
 */

using System.Collections.Generic;

namespace Abelkhan
{
    public class svr_msg_handle
    {
        private center_module _center_module;
        private SvrManager _svrmng;
        private CloseHandle _closehandle;

        public svr_msg_handle(SvrManager svrs, CloseHandle closehandle)
        {
            _svrmng = svrs;
            _closehandle = closehandle;

            _center_module = new center_module(Abelkhan.ModuleMgrHandle._modulemng);
            _center_module.on_reg_server_mq += reg_server_mq;
            _center_module.on_reconn_reg_server_mq += on_reconn_reg_server_mq;
            _center_module.on_heartbeat += heartbeat;
            _center_module.on_closed += closed;
        }

        private void reg_server_mq(string type, string hub_type, string svr_name)
        {
            var rsp = (Abelkhan.center_reg_server_mq_rsp)_center_module.rsp.Value;
            rsp.rsp();

            _svrmng.for_each_hub((HubProxy _proxy) => {
                _proxy.distribute_server_mq(type, svr_name);
            });

            if (type == "hub")
            {
                var _hubproxy = _svrmng.reg_hub(_center_module.current_ch.Value, hub_type, svr_name);

                _svrmng.for_each_svr((SvrProxy _proxy) => {
                    _hubproxy.distribute_server_mq(_proxy.type, _proxy.name);
                });
            }

            _svrmng.reg_svr(_center_module.current_ch.Value, type, hub_type, svr_name);
        }

        private void on_reconn_reg_server_mq(string type, string hub_type, string svr_name)
        {
            var rsp = (Abelkhan.center_reconn_reg_server_mq_rsp)_center_module.rsp.Value;
            rsp.rsp();

            _svrmng.for_each_new_hub((HubProxy _proxy) => {
                _proxy.distribute_server_mq(type, svr_name);
            });

            if (type == "hub")
            {
                var _hubproxy = _svrmng.reg_hub(_center_module.current_ch.Value, hub_type, svr_name, true);

                _svrmng.for_each_new_svr((SvrProxy _proxy) => {
                    _hubproxy.distribute_server_mq(_proxy.type, _proxy.name);
                });
            }
            _svrmng.reg_svr(_center_module.current_ch.Value, type, hub_type, svr_name, true);
        }

        private void heartbeat(uint tick)
        {
            var rsp = (Abelkhan.center_heartbeat_rsp)_center_module.rsp.Value;
            rsp.rsp();

            if (_svrmng.get_svr(_center_module.current_ch.Value, out SvrProxy _svr_proxy))
            {
                _svr_proxy.timetmp = Service.Timerservice.Tick;
                _svr_proxy.tick = tick;
            }
        }

        private void closed()
        {
            if (_svrmng.get_svr(_center_module.current_ch.Value, out SvrProxy _svr_proxy))
            {
                _svr_proxy.is_closed = true;
                _svr_proxy.closed_svr();
            }

            if (_svrmng.get_hub(_center_module.current_ch.Value, out HubProxy _hub_proxy))
            {
                _hub_proxy.is_closed = true;
            }

            if (_closehandle.is_closing && _svrmng.check_all_hub_closed())
            {
                _svrmng.close_db();

                if (_svrmng.check_all_db_closed())
                {
                    _closehandle.is_close = true;
                }
            }
        }
    }
}