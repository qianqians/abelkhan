/*
 * gm_msg_handle
 * 2020/6/2
 * qianqians
 */

namespace Abelkhan
{
    public class gm_msg_handle
    {
        private Abelkhan.gm_center_module gm_center_module;
        private SvrManager svrmng;
        private GMManager gmmng;
        private CloseHandle closeHandle;

        public gm_msg_handle(SvrManager svrs, GMManager gms, CloseHandle _closeHandle)
        {
            svrmng = svrs;
            gmmng = gms;
            closeHandle = _closeHandle;

            gm_center_module = new Abelkhan.gm_center_module(Abelkhan.ModuleMgrHandle._modulemng);
            gm_center_module.on_confirm_gm += confirm_gm;
            gm_center_module.on_close_clutter += close_clutter;
            gm_center_module.on_reload += reload;
        }

        private void confirm_gm(string gm_name)
        {
            try
            {
                gmmng.reg_gm(gm_name, gm_center_module.current_ch.Value);
            }
            catch (System.Exception e)
            {
                Log.Log.err("confirm_gm {0} {1}", gm_name, e.Message);
            }
        }

        private void close_clutter(string gmname)
        {
            try
            {
                if (gmmng.check_gm(gmname, gm_center_module.current_ch.Value))
                {
                    Log.Log.trace("close_clutter {0}", gmname);

                    closeHandle.is_closing = true;
                    svrmng.for_each_svr((SvrProxy _svrproxy) =>
                    {
                        _svrproxy.close_server();
                    });

                    if (svrmng.check_all_hub_closed())
                    {
                        closeHandle.is_close = true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Log.err("close_clutter {0} {1}", gmname, e.Message);
            }
        }

        private void reload(string gmname, string argvs)
        {
            try
            {
                if (gmmng.check_gm(gmname, gm_center_module.current_ch.Value))
                {
                    svrmng.for_each_hub((HubProxy _proxy) =>
                    {
                        _proxy.reload(argvs);
                    });
                }
            }
            catch (System.Exception e)
            {
                Log.Log.err("reload {0} {1}", gmname, e.Message);
            }
        }
    }
}