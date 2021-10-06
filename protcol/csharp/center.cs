using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
/*this caller code is codegen by abelkhan codegen for c#*/
/*this cb code is codegen by abelkhan for c#*/
    public class center_call_hub_rsp_cb : abelkhan.Imodule {
        public center_call_hub_rsp_cb(abelkhan.modulemng modules) : base("center_call_hub_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class center_call_hub_caller : abelkhan.Icaller {
        public static center_call_hub_rsp_cb rsp_cb_center_call_hub_handle = null;
        private Int64 uuid_adbd1e34_0c90_3426_aefa_4d734c07a706 = (Int64)RandomUUID.random();

        public center_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("center_call_hub", _ch)
        {
            if (rsp_cb_center_call_hub_handle == null)
            {
                rsp_cb_center_call_hub_handle = new center_call_hub_rsp_cb(modules);
            }
        }

        public void distribute_server_address(string svr_type, string svr_name, string ip, UInt16 port){
            var _argv_b71bf35c_d65b_3682_98d1_b934f5276558 = new ArrayList();
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.Add(svr_type);
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.Add(svr_name);
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.Add(ip);
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.Add(port);
            call_module_method("distribute_server_address", _argv_b71bf35c_d65b_3682_98d1_b934f5276558);
        }

        public void reload(string argv){
            var _argv_ba37af53_beea_3d61_82e1_8d15e335971d = new ArrayList();
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.Add(argv);
            call_module_method("reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class center_call_server_rsp_cb : abelkhan.Imodule {
        public center_call_server_rsp_cb(abelkhan.modulemng modules) : base("center_call_server_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class center_call_server_caller : abelkhan.Icaller {
        public static center_call_server_rsp_cb rsp_cb_center_call_server_handle = null;
        private Int64 uuid_8c11e5bb_e9ff_3a0b_a436_65a9922a8da5 = (Int64)RandomUUID.random();

        public center_call_server_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("center_call_server", _ch)
        {
            if (rsp_cb_center_call_server_handle == null)
            {
                rsp_cb_center_call_server_handle = new center_call_server_rsp_cb(modules);
            }
        }

        public void close_server(){
            var _argv_8394af17_8a06_3068_977d_477a1276f56e = new ArrayList();
            call_module_method("close_server", _argv_8394af17_8a06_3068_977d_477a1276f56e);
        }

        public void svr_be_closed(string svr_type, string svr_name){
            var _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac = new ArrayList();
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.Add(svr_type);
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.Add(svr_name);
            call_module_method("svr_be_closed", _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac);
        }

    }
    public class center_reg_server_cb
    {
        private UInt64 cb_uuid;
        private center_rsp_cb module_rsp_cb;

        public center_reg_server_cb(UInt64 _cb_uuid, center_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reg_server_cb;
        public event Action on_reg_server_err;
        public event Action on_reg_server_timeout;

        public center_reg_server_cb callBack(Action cb, Action err)
        {
            on_reg_server_cb += cb;
            on_reg_server_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.reg_server_timeout(cb_uuid);
            });
            on_reg_server_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_reg_server_cb != null)
            {
                on_reg_server_cb();
            }
        }

        public void call_err()
        {
            if (on_reg_server_err != null)
            {
                on_reg_server_err();
            }
        }

        public void call_timeout()
        {
            if (on_reg_server_timeout != null)
            {
                on_reg_server_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class center_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, center_reg_server_cb> map_reg_server;
        public center_rsp_cb(abelkhan.modulemng modules) : base("center_rsp_cb")
        {
            modules.reg_module(this);
            map_reg_server = new Dictionary<UInt64, center_reg_server_cb>();
            reg_method("reg_server_rsp", reg_server_rsp);
            reg_method("reg_server_err", reg_server_err);
        }

        public void reg_server_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_reg_server_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reg_server_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_reg_server_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void reg_server_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_reg_server_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private center_reg_server_cb try_get_and_del_reg_server_cb(UInt64 uuid){
            lock(map_reg_server)
            {
                var rsp = map_reg_server[uuid];
                map_reg_server.Remove(uuid);
                return rsp;
            }
        }

    }

    public class center_caller : abelkhan.Icaller {
        public static center_rsp_cb rsp_cb_center_handle = null;
        private Int64 uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 = (Int64)RandomUUID.random();

        public center_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("center", _ch)
        {
            if (rsp_cb_center_handle == null)
            {
                rsp_cb_center_handle = new center_rsp_cb(modules);
            }
        }

        public center_reg_server_cb reg_server(string type, string ip, UInt16 port, string svr_name){
            Interlocked.Increment(ref uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066);
            var uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255 = (UInt64)uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066;

            var _argv_86ab8166_c1a7_3809_8c9b_df444f746076 = new ArrayList();
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255);
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(type);
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(ip);
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(port);
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(svr_name);
            call_module_method("reg_server", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);

            var cb_reg_server_obj = new center_reg_server_cb(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255, rsp_cb_center_handle);
            rsp_cb_center_handle.map_reg_server.Add(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255, cb_reg_server_obj);
            return cb_reg_server_obj;
        }

        public void heartbeat(){
            var _argv_af04a217_eafb_393c_9e34_0303485bef77 = new ArrayList();
            call_module_method("heartbeat", _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }

        public void closed(){
            var _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee = new ArrayList();
            call_module_method("closed", _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee);
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class gm_center_rsp_cb : abelkhan.Imodule {
        public gm_center_rsp_cb(abelkhan.modulemng modules) : base("gm_center_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class gm_center_caller : abelkhan.Icaller {
        public static gm_center_rsp_cb rsp_cb_gm_center_handle = null;
        private Int64 uuid_130fb971_5ae0_3446_b480_f9ee83dbeb28 = (Int64)RandomUUID.random();

        public gm_center_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("gm_center", _ch)
        {
            if (rsp_cb_gm_center_handle == null)
            {
                rsp_cb_gm_center_handle = new gm_center_rsp_cb(modules);
            }
        }

        public void confirm_gm(string gm_name){
            var _argv_d097689c_b711_3837_8783_64916ae34cea = new ArrayList();
            _argv_d097689c_b711_3837_8783_64916ae34cea.Add(gm_name);
            call_module_method("confirm_gm", _argv_d097689c_b711_3837_8783_64916ae34cea);
        }

        public void close_clutter(string gmname){
            var _argv_576c03c3_06da_36a2_b868_752da601cb54 = new ArrayList();
            _argv_576c03c3_06da_36a2_b868_752da601cb54.Add(gmname);
            call_module_method("close_clutter", _argv_576c03c3_06da_36a2_b868_752da601cb54);
        }

        public void reload(string gmname, string argv){
            var _argv_ba37af53_beea_3d61_82e1_8d15e335971d = new ArrayList();
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.Add(gmname);
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.Add(argv);
            call_module_method("reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class center_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public center_call_hub_module(abelkhan.modulemng _modules) : base("center_call_hub")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("distribute_server_address", distribute_server_address);
            reg_method("reload", reload);
        }

        public event Action<string, string, string, UInt16> on_distribute_server_address;
        public void distribute_server_address(ArrayList inArray){
            var _svr_type = (string)inArray[0];
            var _svr_name = (string)inArray[1];
            var _ip = (string)inArray[2];
            var _port = (UInt16)inArray[3];
            if (on_distribute_server_address != null){
                on_distribute_server_address(_svr_type, _svr_name, _ip, _port);
            }
        }

        public event Action<string> on_reload;
        public void reload(ArrayList inArray){
            var _argv = (string)inArray[0];
            if (on_reload != null){
                on_reload(_argv);
            }
        }

    }
    public class center_call_server_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public center_call_server_module(abelkhan.modulemng _modules) : base("center_call_server")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("close_server", close_server);
            reg_method("svr_be_closed", svr_be_closed);
        }

        public event Action on_close_server;
        public void close_server(ArrayList inArray){
            if (on_close_server != null){
                on_close_server();
            }
        }

        public event Action<string, string> on_svr_be_closed;
        public void svr_be_closed(ArrayList inArray){
            var _svr_type = (string)inArray[0];
            var _svr_name = (string)inArray[1];
            if (on_svr_be_closed != null){
                on_svr_be_closed(_svr_type, _svr_name);
            }
        }

    }
    public class center_reg_server_rsp : abelkhan.Response {
        private UInt64 uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda;
        public center_reg_server_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("center_rsp_cb", _ch)
        {
            uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda = _uuid;
        }

        public void rsp(){
            var _argv_86ab8166_c1a7_3809_8c9b_df444f746076 = new ArrayList();
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda);
            call_module_method("reg_server_rsp", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
        }

        public void err(){
            var _argv_86ab8166_c1a7_3809_8c9b_df444f746076 = new ArrayList();
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.Add(uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda);
            call_module_method("reg_server_err", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
        }

    }

    public class center_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public center_module(abelkhan.modulemng _modules) : base("center")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("reg_server", reg_server);
            reg_method("heartbeat", heartbeat);
            reg_method("closed", closed);
        }

        public event Action<string, string, UInt16, string> on_reg_server;
        public void reg_server(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _type = (string)inArray[1];
            var _ip = (string)inArray[2];
            var _port = (UInt16)inArray[3];
            var _svr_name = (string)inArray[4];
            rsp = new center_reg_server_rsp(current_ch, _cb_uuid);
            if (on_reg_server != null){
                on_reg_server(_type, _ip, _port, _svr_name);
            }
            rsp = null;
        }

        public event Action on_heartbeat;
        public void heartbeat(ArrayList inArray){
            if (on_heartbeat != null){
                on_heartbeat();
            }
        }

        public event Action on_closed;
        public void closed(ArrayList inArray){
            if (on_closed != null){
                on_closed();
            }
        }

    }
    public class gm_center_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public gm_center_module(abelkhan.modulemng _modules) : base("gm_center")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("confirm_gm", confirm_gm);
            reg_method("close_clutter", close_clutter);
            reg_method("reload", reload);
        }

        public event Action<string> on_confirm_gm;
        public void confirm_gm(ArrayList inArray){
            var _gm_name = (string)inArray[0];
            if (on_confirm_gm != null){
                on_confirm_gm(_gm_name);
            }
        }

        public event Action<string> on_close_clutter;
        public void close_clutter(ArrayList inArray){
            var _gmname = (string)inArray[0];
            if (on_close_clutter != null){
                on_close_clutter(_gmname);
            }
        }

        public event Action<string, string> on_reload;
        public void reload(ArrayList inArray){
            var _gmname = (string)inArray[0];
            var _argv = (string)inArray[1];
            if (on_reload != null){
                on_reload(_gmname, _argv);
            }
        }

    }

}
