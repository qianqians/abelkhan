using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by Abelkhan codegen for c#*/

/*this struct code is codegen by Abelkhan codegen for c#*/
/*this caller code is codegen by Abelkhan codegen for c#*/
    public class center_reg_server_mq_cb
    {
        private UInt64 cb_uuid;
        private center_rsp_cb module_rsp_cb;

        public center_reg_server_mq_cb(UInt64 _cb_uuid, center_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reg_server_mq_cb;
        public event Action on_reg_server_mq_err;
        public event Action on_reg_server_mq_timeout;

        public center_reg_server_mq_cb callBack(Action cb, Action err)
        {
            on_reg_server_mq_cb += cb;
            on_reg_server_mq_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.reg_server_mq_timeout(cb_uuid);
            });
            on_reg_server_mq_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_reg_server_mq_cb != null)
            {
                on_reg_server_mq_cb();
            }
        }

        public void call_err()
        {
            if (on_reg_server_mq_err != null)
            {
                on_reg_server_mq_err();
            }
        }

        public void call_timeout()
        {
            if (on_reg_server_mq_timeout != null)
            {
                on_reg_server_mq_timeout();
            }
        }

    }

    public class center_reconn_reg_server_mq_cb
    {
        private UInt64 cb_uuid;
        private center_rsp_cb module_rsp_cb;

        public center_reconn_reg_server_mq_cb(UInt64 _cb_uuid, center_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reconn_reg_server_mq_cb;
        public event Action on_reconn_reg_server_mq_err;
        public event Action on_reconn_reg_server_mq_timeout;

        public center_reconn_reg_server_mq_cb callBack(Action cb, Action err)
        {
            on_reconn_reg_server_mq_cb += cb;
            on_reconn_reg_server_mq_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.reconn_reg_server_mq_timeout(cb_uuid);
            });
            on_reconn_reg_server_mq_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_reconn_reg_server_mq_cb != null)
            {
                on_reconn_reg_server_mq_cb();
            }
        }

        public void call_err()
        {
            if (on_reconn_reg_server_mq_err != null)
            {
                on_reconn_reg_server_mq_err();
            }
        }

        public void call_timeout()
        {
            if (on_reconn_reg_server_mq_timeout != null)
            {
                on_reconn_reg_server_mq_timeout();
            }
        }

    }

    public class center_heartbeat_cb
    {
        private UInt64 cb_uuid;
        private center_rsp_cb module_rsp_cb;

        public center_heartbeat_cb(UInt64 _cb_uuid, center_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_heartbeat_cb;
        public event Action on_heartbeat_err;
        public event Action on_heartbeat_timeout;

        public center_heartbeat_cb callBack(Action cb, Action err)
        {
            on_heartbeat_cb += cb;
            on_heartbeat_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.heartbeat_timeout(cb_uuid);
            });
            on_heartbeat_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_heartbeat_cb != null)
            {
                on_heartbeat_cb();
            }
        }

        public void call_err()
        {
            if (on_heartbeat_err != null)
            {
                on_heartbeat_err();
            }
        }

        public void call_timeout()
        {
            if (on_heartbeat_timeout != null)
            {
                on_heartbeat_timeout();
            }
        }

    }

/*this cb code is codegen by Abelkhan for c#*/
    public class center_rsp_cb : Abelkhan.Imodule {
        public Dictionary<UInt64, center_reg_server_mq_cb> map_reg_server_mq;
        public Dictionary<UInt64, center_reconn_reg_server_mq_cb> map_reconn_reg_server_mq;
        public Dictionary<UInt64, center_heartbeat_cb> map_heartbeat;
        public center_rsp_cb(Abelkhan.modulemng modules) : base("center_rsp_cb")
        {
            map_reg_server_mq = new Dictionary<UInt64, center_reg_server_mq_cb>();
            modules.reg_method("center_rsp_cb_reg_server_mq_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reg_server_mq_rsp));
            modules.reg_method("center_rsp_cb_reg_server_mq_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reg_server_mq_err));
            map_reconn_reg_server_mq = new Dictionary<UInt64, center_reconn_reg_server_mq_cb>();
            modules.reg_method("center_rsp_cb_reconn_reg_server_mq_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reconn_reg_server_mq_rsp));
            modules.reg_method("center_rsp_cb_reconn_reg_server_mq_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reconn_reg_server_mq_err));
            map_heartbeat = new Dictionary<UInt64, center_heartbeat_cb>();
            modules.reg_method("center_rsp_cb_heartbeat_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, heartbeat_rsp));
            modules.reg_method("center_rsp_cb_heartbeat_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, heartbeat_err));
        }

        public void reg_server_mq_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reg_server_mq_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reg_server_mq_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reg_server_mq_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void reg_server_mq_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_reg_server_mq_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private center_reg_server_mq_cb try_get_and_del_reg_server_mq_cb(UInt64 uuid){
            lock(map_reg_server_mq)
            {
                if (map_reg_server_mq.TryGetValue(uuid, out center_reg_server_mq_cb rsp))
                {
                    map_reg_server_mq.Remove(uuid);
                }
                return rsp;
            }
        }

        public void reconn_reg_server_mq_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reconn_reg_server_mq_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reconn_reg_server_mq_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reconn_reg_server_mq_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void reconn_reg_server_mq_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_reconn_reg_server_mq_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private center_reconn_reg_server_mq_cb try_get_and_del_reconn_reg_server_mq_cb(UInt64 uuid){
            lock(map_reconn_reg_server_mq)
            {
                if (map_reconn_reg_server_mq.TryGetValue(uuid, out center_reconn_reg_server_mq_cb rsp))
                {
                    map_reconn_reg_server_mq.Remove(uuid);
                }
                return rsp;
            }
        }

        public void heartbeat_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_heartbeat_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void heartbeat_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_heartbeat_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void heartbeat_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_heartbeat_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private center_heartbeat_cb try_get_and_del_heartbeat_cb(UInt64 uuid){
            lock(map_heartbeat)
            {
                if (map_heartbeat.TryGetValue(uuid, out center_heartbeat_cb rsp))
                {
                    map_heartbeat.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class center_caller : Abelkhan.Icaller {
        public static center_rsp_cb rsp_cb_center_handle = null;
        private Int32 uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 = (Int32)RandomUUID.random();

        public center_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("center", _ch)
        {
            if (rsp_cb_center_handle == null)
            {
                rsp_cb_center_handle = new center_rsp_cb(modules);
            }
        }

        public center_reg_server_mq_cb reg_server_mq(string type, string hub_type, string svr_name){
            var uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf = (UInt32)Interlocked.Increment(ref uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066);

            var _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2 = new ArrayList();
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.Add(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf);
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.Add(type);
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.Add(hub_type);
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.Add(svr_name);
            call_module_method("center_reg_server_mq", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);

            var cb_reg_server_mq_obj = new center_reg_server_mq_cb(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, rsp_cb_center_handle);
            lock(rsp_cb_center_handle.map_reg_server_mq)
            {
                rsp_cb_center_handle.map_reg_server_mq.Add(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, cb_reg_server_mq_obj);
            }
            return cb_reg_server_mq_obj;
        }

        public center_reconn_reg_server_mq_cb reconn_reg_server_mq(string type, string hub_type, string svr_name){
            var uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21 = (UInt32)Interlocked.Increment(ref uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066);

            var _argv_a018be20_2048_315d_9832_8120b194980f = new ArrayList();
            _argv_a018be20_2048_315d_9832_8120b194980f.Add(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21);
            _argv_a018be20_2048_315d_9832_8120b194980f.Add(type);
            _argv_a018be20_2048_315d_9832_8120b194980f.Add(hub_type);
            _argv_a018be20_2048_315d_9832_8120b194980f.Add(svr_name);
            call_module_method("center_reconn_reg_server_mq", _argv_a018be20_2048_315d_9832_8120b194980f);

            var cb_reconn_reg_server_mq_obj = new center_reconn_reg_server_mq_cb(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, rsp_cb_center_handle);
            lock(rsp_cb_center_handle.map_reconn_reg_server_mq)
            {
                rsp_cb_center_handle.map_reconn_reg_server_mq.Add(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, cb_reconn_reg_server_mq_obj);
            }
            return cb_reconn_reg_server_mq_obj;
        }

        public center_heartbeat_cb heartbeat(UInt32 tick){
            var uuid_9654538a_9916_57dc_8ea5_806086d7a378 = (UInt32)Interlocked.Increment(ref uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066);

            var _argv_af04a217_eafb_393c_9e34_0303485bef77 = new ArrayList();
            _argv_af04a217_eafb_393c_9e34_0303485bef77.Add(uuid_9654538a_9916_57dc_8ea5_806086d7a378);
            _argv_af04a217_eafb_393c_9e34_0303485bef77.Add(tick);
            call_module_method("center_heartbeat", _argv_af04a217_eafb_393c_9e34_0303485bef77);

            var cb_heartbeat_obj = new center_heartbeat_cb(uuid_9654538a_9916_57dc_8ea5_806086d7a378, rsp_cb_center_handle);
            lock(rsp_cb_center_handle.map_heartbeat)
            {
                rsp_cb_center_handle.map_heartbeat.Add(uuid_9654538a_9916_57dc_8ea5_806086d7a378, cb_heartbeat_obj);
            }
            return cb_heartbeat_obj;
        }

        public void closed(){
            var _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee = new ArrayList();
            call_module_method("center_closed", _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee);
        }

    }
/*this cb code is codegen by Abelkhan for c#*/
    public class center_call_server_rsp_cb : Abelkhan.Imodule {
        public center_call_server_rsp_cb(Abelkhan.modulemng modules) : base("center_call_server_rsp_cb")
        {
        }

    }

    public class center_call_server_caller : Abelkhan.Icaller {
        public static center_call_server_rsp_cb rsp_cb_center_call_server_handle = null;
        private Int32 uuid_8c11e5bb_e9ff_3a0b_a436_65a9922a8da5 = (Int32)RandomUUID.random();

        public center_call_server_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("center_call_server", _ch)
        {
            if (rsp_cb_center_call_server_handle == null)
            {
                rsp_cb_center_call_server_handle = new center_call_server_rsp_cb(modules);
            }
        }

        public void close_server(){
            var _argv_8394af17_8a06_3068_977d_477a1276f56e = new ArrayList();
            call_module_method("center_call_server_close_server", _argv_8394af17_8a06_3068_977d_477a1276f56e);
        }

        public void console_close_server(string svr_type, string svr_name){
            var _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc = new ArrayList();
            _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.Add(svr_type);
            _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.Add(svr_name);
            call_module_method("center_call_server_console_close_server", _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc);
        }

        public void svr_be_closed(string svr_type, string svr_name){
            var _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac = new ArrayList();
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.Add(svr_type);
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.Add(svr_name);
            call_module_method("center_call_server_svr_be_closed", _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac);
        }

        public void take_over_svr(string svr_name){
            var _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4 = new ArrayList();
            _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4.Add(svr_name);
            call_module_method("center_call_server_take_over_svr", _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4);
        }

    }
/*this cb code is codegen by Abelkhan for c#*/
    public class center_call_hub_rsp_cb : Abelkhan.Imodule {
        public center_call_hub_rsp_cb(Abelkhan.modulemng modules) : base("center_call_hub_rsp_cb")
        {
        }

    }

    public class center_call_hub_caller : Abelkhan.Icaller {
        public static center_call_hub_rsp_cb rsp_cb_center_call_hub_handle = null;
        private Int32 uuid_adbd1e34_0c90_3426_aefa_4d734c07a706 = (Int32)RandomUUID.random();

        public center_call_hub_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("center_call_hub", _ch)
        {
            if (rsp_cb_center_call_hub_handle == null)
            {
                rsp_cb_center_call_hub_handle = new center_call_hub_rsp_cb(modules);
            }
        }

        public void distribute_server_mq(string svr_type, string svr_name){
            var _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393 = new ArrayList();
            _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.Add(svr_type);
            _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.Add(svr_name);
            call_module_method("center_call_hub_distribute_server_mq", _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393);
        }

        public void reload(string argv){
            var _argv_ba37af53_beea_3d61_82e1_8d15e335971d = new ArrayList();
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.Add(argv);
            call_module_method("center_call_hub_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    }
/*this cb code is codegen by Abelkhan for c#*/
    public class gm_center_rsp_cb : Abelkhan.Imodule {
        public gm_center_rsp_cb(Abelkhan.modulemng modules) : base("gm_center_rsp_cb")
        {
        }

    }

    public class gm_center_caller : Abelkhan.Icaller {
        public static gm_center_rsp_cb rsp_cb_gm_center_handle = null;
        private Int32 uuid_130fb971_5ae0_3446_b480_f9ee83dbeb28 = (Int32)RandomUUID.random();

        public gm_center_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("gm_center", _ch)
        {
            if (rsp_cb_gm_center_handle == null)
            {
                rsp_cb_gm_center_handle = new gm_center_rsp_cb(modules);
            }
        }

        public void confirm_gm(string gm_name){
            var _argv_d097689c_b711_3837_8783_64916ae34cea = new ArrayList();
            _argv_d097689c_b711_3837_8783_64916ae34cea.Add(gm_name);
            call_module_method("gm_center_confirm_gm", _argv_d097689c_b711_3837_8783_64916ae34cea);
        }

        public void close_clutter(string gmname){
            var _argv_576c03c3_06da_36a2_b868_752da601cb54 = new ArrayList();
            _argv_576c03c3_06da_36a2_b868_752da601cb54.Add(gmname);
            call_module_method("gm_center_close_clutter", _argv_576c03c3_06da_36a2_b868_752da601cb54);
        }

        public void reload(string gmname, string argv){
            var _argv_ba37af53_beea_3d61_82e1_8d15e335971d = new ArrayList();
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.Add(gmname);
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.Add(argv);
            call_module_method("gm_center_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    }
/*this module code is codegen by Abelkhan codegen for c#*/
    public class center_reg_server_mq_rsp : Abelkhan.Response {
        private UInt64 uuid_7254d987_ac9c_3d73_831c_f43efb3268a9;
        public center_reg_server_mq_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("center_rsp_cb", _ch)
        {
            uuid_7254d987_ac9c_3d73_831c_f43efb3268a9 = _uuid;
        }

        public void rsp(){
            var _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2 = new ArrayList();
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.Add(uuid_7254d987_ac9c_3d73_831c_f43efb3268a9);
            call_module_method("center_rsp_cb_reg_server_mq_rsp", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
        }

        public void err(){
            var _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2 = new ArrayList();
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.Add(uuid_7254d987_ac9c_3d73_831c_f43efb3268a9);
            call_module_method("center_rsp_cb_reg_server_mq_err", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
        }

    }

    public class center_reconn_reg_server_mq_rsp : Abelkhan.Response {
        private UInt64 uuid_4d058274_a122_382e_8084_b9067ed713c5;
        public center_reconn_reg_server_mq_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("center_rsp_cb", _ch)
        {
            uuid_4d058274_a122_382e_8084_b9067ed713c5 = _uuid;
        }

        public void rsp(){
            var _argv_a018be20_2048_315d_9832_8120b194980f = new ArrayList();
            _argv_a018be20_2048_315d_9832_8120b194980f.Add(uuid_4d058274_a122_382e_8084_b9067ed713c5);
            call_module_method("center_rsp_cb_reconn_reg_server_mq_rsp", _argv_a018be20_2048_315d_9832_8120b194980f);
        }

        public void err(){
            var _argv_a018be20_2048_315d_9832_8120b194980f = new ArrayList();
            _argv_a018be20_2048_315d_9832_8120b194980f.Add(uuid_4d058274_a122_382e_8084_b9067ed713c5);
            call_module_method("center_rsp_cb_reconn_reg_server_mq_err", _argv_a018be20_2048_315d_9832_8120b194980f);
        }

    }

    public class center_heartbeat_rsp : Abelkhan.Response {
        private UInt64 uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f;
        public center_heartbeat_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("center_rsp_cb", _ch)
        {
            uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f = _uuid;
        }

        public void rsp(){
            var _argv_af04a217_eafb_393c_9e34_0303485bef77 = new ArrayList();
            _argv_af04a217_eafb_393c_9e34_0303485bef77.Add(uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f);
            call_module_method("center_rsp_cb_heartbeat_rsp", _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }

        public void err(){
            var _argv_af04a217_eafb_393c_9e34_0303485bef77 = new ArrayList();
            _argv_af04a217_eafb_393c_9e34_0303485bef77.Add(uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f);
            call_module_method("center_rsp_cb_heartbeat_err", _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }

    }

    public class center_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public center_module(Abelkhan.modulemng _modules) : base("center")
        {
            modules = _modules;
            modules.reg_method("center_reg_server_mq", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reg_server_mq));
            modules.reg_method("center_reconn_reg_server_mq", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reconn_reg_server_mq));
            modules.reg_method("center_heartbeat", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, heartbeat));
            modules.reg_method("center_closed", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, closed));
        }

        public event Action<string, string, string> on_reg_server_mq;
        public void reg_server_mq(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _type = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _hub_type = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _svr_name = ((MsgPack.MessagePackObject)inArray[3]).AsString();
            rsp.Value = new center_reg_server_mq_rsp(current_ch.Value, _cb_uuid);
            if (on_reg_server_mq != null){
                on_reg_server_mq(_type, _hub_type, _svr_name);
            }
            rsp.Value = null;
        }

        public event Action<string, string, string> on_reconn_reg_server_mq;
        public void reconn_reg_server_mq(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _type = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _hub_type = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _svr_name = ((MsgPack.MessagePackObject)inArray[3]).AsString();
            rsp.Value = new center_reconn_reg_server_mq_rsp(current_ch.Value, _cb_uuid);
            if (on_reconn_reg_server_mq != null){
                on_reconn_reg_server_mq(_type, _hub_type, _svr_name);
            }
            rsp.Value = null;
        }

        public event Action<UInt32> on_heartbeat;
        public void heartbeat(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _tick = ((MsgPack.MessagePackObject)inArray[1]).AsUInt32();
            rsp.Value = new center_heartbeat_rsp(current_ch.Value, _cb_uuid);
            if (on_heartbeat != null){
                on_heartbeat(_tick);
            }
            rsp.Value = null;
        }

        public event Action on_closed;
        public void closed(IList<MsgPack.MessagePackObject> inArray){
            if (on_closed != null){
                on_closed();
            }
        }

    }
    public class center_call_server_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public center_call_server_module(Abelkhan.modulemng _modules) : base("center_call_server")
        {
            modules = _modules;
            modules.reg_method("center_call_server_close_server", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, close_server));
            modules.reg_method("center_call_server_console_close_server", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, console_close_server));
            modules.reg_method("center_call_server_svr_be_closed", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, svr_be_closed));
            modules.reg_method("center_call_server_take_over_svr", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, take_over_svr));
        }

        public event Action on_close_server;
        public void close_server(IList<MsgPack.MessagePackObject> inArray){
            if (on_close_server != null){
                on_close_server();
            }
        }

        public event Action<string, string> on_console_close_server;
        public void console_close_server(IList<MsgPack.MessagePackObject> inArray){
            var _svr_type = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _svr_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            if (on_console_close_server != null){
                on_console_close_server(_svr_type, _svr_name);
            }
        }

        public event Action<string, string> on_svr_be_closed;
        public void svr_be_closed(IList<MsgPack.MessagePackObject> inArray){
            var _svr_type = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _svr_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            if (on_svr_be_closed != null){
                on_svr_be_closed(_svr_type, _svr_name);
            }
        }

        public event Action<string> on_take_over_svr;
        public void take_over_svr(IList<MsgPack.MessagePackObject> inArray){
            var _svr_name = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_take_over_svr != null){
                on_take_over_svr(_svr_name);
            }
        }

    }
    public class center_call_hub_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public center_call_hub_module(Abelkhan.modulemng _modules) : base("center_call_hub")
        {
            modules = _modules;
            modules.reg_method("center_call_hub_distribute_server_mq", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, distribute_server_mq));
            modules.reg_method("center_call_hub_reload", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reload));
        }

        public event Action<string, string> on_distribute_server_mq;
        public void distribute_server_mq(IList<MsgPack.MessagePackObject> inArray){
            var _svr_type = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _svr_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            if (on_distribute_server_mq != null){
                on_distribute_server_mq(_svr_type, _svr_name);
            }
        }

        public event Action<string> on_reload;
        public void reload(IList<MsgPack.MessagePackObject> inArray){
            var _argv = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_reload != null){
                on_reload(_argv);
            }
        }

    }
    public class gm_center_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public gm_center_module(Abelkhan.modulemng _modules) : base("gm_center")
        {
            modules = _modules;
            modules.reg_method("gm_center_confirm_gm", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, confirm_gm));
            modules.reg_method("gm_center_close_clutter", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, close_clutter));
            modules.reg_method("gm_center_reload", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reload));
        }

        public event Action<string> on_confirm_gm;
        public void confirm_gm(IList<MsgPack.MessagePackObject> inArray){
            var _gm_name = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_confirm_gm != null){
                on_confirm_gm(_gm_name);
            }
        }

        public event Action<string> on_close_clutter;
        public void close_clutter(IList<MsgPack.MessagePackObject> inArray){
            var _gmname = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_close_clutter != null){
                on_close_clutter(_gmname);
            }
        }

        public event Action<string, string> on_reload;
        public void reload(IList<MsgPack.MessagePackObject> inArray){
            var _gmname = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _argv = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            if (on_reload != null){
                on_reload(_gmname, _argv);
            }
        }

    }

}
