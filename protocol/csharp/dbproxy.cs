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
    public class hub_call_dbproxy_reg_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_reg_hub_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reg_hub_cb;
        public event Action on_reg_hub_err;
        public event Action on_reg_hub_timeout;

        public hub_call_dbproxy_reg_hub_cb callBack(Action cb, Action err)
        {
            on_reg_hub_cb += cb;
            on_reg_hub_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.reg_hub_timeout(cb_uuid);
            });
            on_reg_hub_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_reg_hub_cb != null)
            {
                on_reg_hub_cb();
            }
        }

        public void call_err()
        {
            if (on_reg_hub_err != null)
            {
                on_reg_hub_err();
            }
        }

        public void call_timeout()
        {
            if (on_reg_hub_timeout != null)
            {
                on_reg_hub_timeout();
            }
        }

    }

    public class hub_call_dbproxy_get_guid_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_get_guid_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<Int64> on_get_guid_cb;
        public event Action on_get_guid_err;
        public event Action on_get_guid_timeout;

        public hub_call_dbproxy_get_guid_cb callBack(Action<Int64> cb, Action err)
        {
            on_get_guid_cb += cb;
            on_get_guid_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_guid_timeout(cb_uuid);
            });
            on_get_guid_timeout += timeout_cb;
        }

        public void call_cb(Int64 guid)
        {
            if (on_get_guid_cb != null)
            {
                on_get_guid_cb(guid);
            }
        }

        public void call_err()
        {
            if (on_get_guid_err != null)
            {
                on_get_guid_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_guid_timeout != null)
            {
                on_get_guid_timeout();
            }
        }

    }

    public class hub_call_dbproxy_create_persisted_object_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_create_persisted_object_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_create_persisted_object_cb;
        public event Action on_create_persisted_object_err;
        public event Action on_create_persisted_object_timeout;

        public hub_call_dbproxy_create_persisted_object_cb callBack(Action cb, Action err)
        {
            on_create_persisted_object_cb += cb;
            on_create_persisted_object_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.create_persisted_object_timeout(cb_uuid);
            });
            on_create_persisted_object_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_create_persisted_object_cb != null)
            {
                on_create_persisted_object_cb();
            }
        }

        public void call_err()
        {
            if (on_create_persisted_object_err != null)
            {
                on_create_persisted_object_err();
            }
        }

        public void call_timeout()
        {
            if (on_create_persisted_object_timeout != null)
            {
                on_create_persisted_object_timeout();
            }
        }

    }

    public class hub_call_dbproxy_updata_persisted_object_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_updata_persisted_object_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_updata_persisted_object_cb;
        public event Action on_updata_persisted_object_err;
        public event Action on_updata_persisted_object_timeout;

        public hub_call_dbproxy_updata_persisted_object_cb callBack(Action cb, Action err)
        {
            on_updata_persisted_object_cb += cb;
            on_updata_persisted_object_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.updata_persisted_object_timeout(cb_uuid);
            });
            on_updata_persisted_object_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_updata_persisted_object_cb != null)
            {
                on_updata_persisted_object_cb();
            }
        }

        public void call_err()
        {
            if (on_updata_persisted_object_err != null)
            {
                on_updata_persisted_object_err();
            }
        }

        public void call_timeout()
        {
            if (on_updata_persisted_object_timeout != null)
            {
                on_updata_persisted_object_timeout();
            }
        }

    }

    public class hub_call_dbproxy_find_and_modify_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_find_and_modify_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<byte[]> on_find_and_modify_cb;
        public event Action on_find_and_modify_err;
        public event Action on_find_and_modify_timeout;

        public hub_call_dbproxy_find_and_modify_cb callBack(Action<byte[]> cb, Action err)
        {
            on_find_and_modify_cb += cb;
            on_find_and_modify_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.find_and_modify_timeout(cb_uuid);
            });
            on_find_and_modify_timeout += timeout_cb;
        }

        public void call_cb(byte[] object_info)
        {
            if (on_find_and_modify_cb != null)
            {
                on_find_and_modify_cb(object_info);
            }
        }

        public void call_err()
        {
            if (on_find_and_modify_err != null)
            {
                on_find_and_modify_err();
            }
        }

        public void call_timeout()
        {
            if (on_find_and_modify_timeout != null)
            {
                on_find_and_modify_timeout();
            }
        }

    }

    public class hub_call_dbproxy_remove_object_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_remove_object_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_remove_object_cb;
        public event Action on_remove_object_err;
        public event Action on_remove_object_timeout;

        public hub_call_dbproxy_remove_object_cb callBack(Action cb, Action err)
        {
            on_remove_object_cb += cb;
            on_remove_object_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.remove_object_timeout(cb_uuid);
            });
            on_remove_object_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_remove_object_cb != null)
            {
                on_remove_object_cb();
            }
        }

        public void call_err()
        {
            if (on_remove_object_err != null)
            {
                on_remove_object_err();
            }
        }

        public void call_timeout()
        {
            if (on_remove_object_timeout != null)
            {
                on_remove_object_timeout();
            }
        }

    }

    public class hub_call_dbproxy_get_object_count_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_get_object_count_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<UInt32> on_get_object_count_cb;
        public event Action on_get_object_count_err;
        public event Action on_get_object_count_timeout;

        public hub_call_dbproxy_get_object_count_cb callBack(Action<UInt32> cb, Action err)
        {
            on_get_object_count_cb += cb;
            on_get_object_count_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_object_count_timeout(cb_uuid);
            });
            on_get_object_count_timeout += timeout_cb;
        }

        public void call_cb(UInt32 count)
        {
            if (on_get_object_count_cb != null)
            {
                on_get_object_count_cb(count);
            }
        }

        public void call_err()
        {
            if (on_get_object_count_err != null)
            {
                on_get_object_count_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_object_count_timeout != null)
            {
                on_get_object_count_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class hub_call_dbproxy_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, hub_call_dbproxy_reg_hub_cb> map_reg_hub;
        public Dictionary<UInt64, hub_call_dbproxy_get_guid_cb> map_get_guid;
        public Dictionary<UInt64, hub_call_dbproxy_create_persisted_object_cb> map_create_persisted_object;
        public Dictionary<UInt64, hub_call_dbproxy_updata_persisted_object_cb> map_updata_persisted_object;
        public Dictionary<UInt64, hub_call_dbproxy_find_and_modify_cb> map_find_and_modify;
        public Dictionary<UInt64, hub_call_dbproxy_remove_object_cb> map_remove_object;
        public Dictionary<UInt64, hub_call_dbproxy_get_object_count_cb> map_get_object_count;
        public hub_call_dbproxy_rsp_cb(abelkhan.modulemng modules) : base("hub_call_dbproxy_rsp_cb")
        {
            map_reg_hub = new Dictionary<UInt64, hub_call_dbproxy_reg_hub_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_reg_hub_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, reg_hub_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_reg_hub_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, reg_hub_err));
            map_get_guid = new Dictionary<UInt64, hub_call_dbproxy_get_guid_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_get_guid_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_guid_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_get_guid_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_guid_err));
            map_create_persisted_object = new Dictionary<UInt64, hub_call_dbproxy_create_persisted_object_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_create_persisted_object_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, create_persisted_object_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_create_persisted_object_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, create_persisted_object_err));
            map_updata_persisted_object = new Dictionary<UInt64, hub_call_dbproxy_updata_persisted_object_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, updata_persisted_object_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, updata_persisted_object_err));
            map_find_and_modify = new Dictionary<UInt64, hub_call_dbproxy_find_and_modify_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_find_and_modify_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, find_and_modify_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_find_and_modify_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, find_and_modify_err));
            map_remove_object = new Dictionary<UInt64, hub_call_dbproxy_remove_object_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_remove_object_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, remove_object_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_remove_object_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, remove_object_err));
            map_get_object_count = new Dictionary<UInt64, hub_call_dbproxy_get_object_count_cb>();
            modules.reg_method("hub_call_dbproxy_rsp_cb_get_object_count_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_object_count_rsp));
            modules.reg_method("hub_call_dbproxy_rsp_cb_get_object_count_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_object_count_err));
        }

        public void reg_hub_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reg_hub_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void reg_hub_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_reg_hub_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_reg_hub_cb try_get_and_del_reg_hub_cb(UInt64 uuid){
            lock(map_reg_hub)
            {
                if (map_reg_hub.TryGetValue(uuid, out hub_call_dbproxy_reg_hub_cb rsp))
                {
                    map_reg_hub.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_guid_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _guid = ((MsgPack.MessagePackObject)inArray[1]).AsInt64();
            var rsp = try_get_and_del_get_guid_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_guid);
            }
        }

        public void get_guid_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_guid_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_guid_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_guid_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_get_guid_cb try_get_and_del_get_guid_cb(UInt64 uuid){
            lock(map_get_guid)
            {
                if (map_get_guid.TryGetValue(uuid, out hub_call_dbproxy_get_guid_cb rsp))
                {
                    map_get_guid.Remove(uuid);
                }
                return rsp;
            }
        }

        public void create_persisted_object_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_create_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void create_persisted_object_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_create_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void create_persisted_object_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_create_persisted_object_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_create_persisted_object_cb try_get_and_del_create_persisted_object_cb(UInt64 uuid){
            lock(map_create_persisted_object)
            {
                if (map_create_persisted_object.TryGetValue(uuid, out hub_call_dbproxy_create_persisted_object_cb rsp))
                {
                    map_create_persisted_object.Remove(uuid);
                }
                return rsp;
            }
        }

        public void updata_persisted_object_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_updata_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void updata_persisted_object_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_updata_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void updata_persisted_object_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_updata_persisted_object_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_updata_persisted_object_cb try_get_and_del_updata_persisted_object_cb(UInt64 uuid){
            lock(map_updata_persisted_object)
            {
                if (map_updata_persisted_object.TryGetValue(uuid, out hub_call_dbproxy_updata_persisted_object_cb rsp))
                {
                    map_updata_persisted_object.Remove(uuid);
                }
                return rsp;
            }
        }

        public void find_and_modify_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _object_info = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            var rsp = try_get_and_del_find_and_modify_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_object_info);
            }
        }

        public void find_and_modify_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_find_and_modify_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void find_and_modify_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_find_and_modify_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_find_and_modify_cb try_get_and_del_find_and_modify_cb(UInt64 uuid){
            lock(map_find_and_modify)
            {
                if (map_find_and_modify.TryGetValue(uuid, out hub_call_dbproxy_find_and_modify_cb rsp))
                {
                    map_find_and_modify.Remove(uuid);
                }
                return rsp;
            }
        }

        public void remove_object_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_remove_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void remove_object_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_remove_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void remove_object_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_remove_object_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_remove_object_cb try_get_and_del_remove_object_cb(UInt64 uuid){
            lock(map_remove_object)
            {
                if (map_remove_object.TryGetValue(uuid, out hub_call_dbproxy_remove_object_cb rsp))
                {
                    map_remove_object.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_object_count_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _count = ((MsgPack.MessagePackObject)inArray[1]).AsUInt32();
            var rsp = try_get_and_del_get_object_count_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_count);
            }
        }

        public void get_object_count_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_object_count_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_object_count_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_object_count_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_get_object_count_cb try_get_and_del_get_object_count_cb(UInt64 uuid){
            lock(map_get_object_count)
            {
                if (map_get_object_count.TryGetValue(uuid, out hub_call_dbproxy_get_object_count_cb rsp))
                {
                    map_get_object_count.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class hub_call_dbproxy_caller : abelkhan.Icaller {
        public static hub_call_dbproxy_rsp_cb rsp_cb_hub_call_dbproxy_handle = null;
        private Int32 uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (Int32)RandomUUID.random();

        public hub_call_dbproxy_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_dbproxy", _ch)
        {
            if (rsp_cb_hub_call_dbproxy_handle == null)
            {
                rsp_cb_hub_call_dbproxy_handle = new hub_call_dbproxy_rsp_cb(modules);
            }
        }

        public hub_call_dbproxy_reg_hub_cb reg_hub(string hub_name){
            var uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_name);
            call_module_method("hub_call_dbproxy_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

            var cb_reg_hub_obj = new hub_call_dbproxy_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_reg_hub)
            {
                rsp_cb_hub_call_dbproxy_handle.map_reg_hub.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
            }
            return cb_reg_hub_obj;
        }

        public hub_call_dbproxy_get_guid_cb get_guid(string db, string collection, string guid_key){
            var uuid_efe126e5_91e4_5df4_975c_18c91b6a6634 = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_8b362c4a_74a5_366e_a6af_37474d7fa521 = new ArrayList();
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(uuid_efe126e5_91e4_5df4_975c_18c91b6a6634);
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(db);
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(collection);
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(guid_key);
            call_module_method("hub_call_dbproxy_get_guid", _argv_8b362c4a_74a5_366e_a6af_37474d7fa521);

            var cb_get_guid_obj = new hub_call_dbproxy_get_guid_cb(uuid_efe126e5_91e4_5df4_975c_18c91b6a6634, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_get_guid)
            {
                rsp_cb_hub_call_dbproxy_handle.map_get_guid.Add(uuid_efe126e5_91e4_5df4_975c_18c91b6a6634, cb_get_guid_obj);
            }
            return cb_get_guid_obj;
        }

        public hub_call_dbproxy_create_persisted_object_cb create_persisted_object(string db, string collection, byte[] object_info){
            var uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = new ArrayList();
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb);
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(db);
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(collection);
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(object_info);
            call_module_method("hub_call_dbproxy_create_persisted_object", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);

            var cb_create_persisted_object_obj = new hub_call_dbproxy_create_persisted_object_cb(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_create_persisted_object)
            {
                rsp_cb_hub_call_dbproxy_handle.map_create_persisted_object.Add(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, cb_create_persisted_object_obj);
            }
            return cb_create_persisted_object_obj;
        }

        public hub_call_dbproxy_updata_persisted_object_cb updata_persisted_object(string db, string collection, byte[] query_info, byte[] updata_info, bool _upsert){
            var uuid_7864a402_2d75_5c02_b24b_50287a06732f = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = new ArrayList();
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(uuid_7864a402_2d75_5c02_b24b_50287a06732f);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(db);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(collection);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(query_info);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(updata_info);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(_upsert);
            call_module_method("hub_call_dbproxy_updata_persisted_object", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);

            var cb_updata_persisted_object_obj = new hub_call_dbproxy_updata_persisted_object_cb(uuid_7864a402_2d75_5c02_b24b_50287a06732f, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_updata_persisted_object)
            {
                rsp_cb_hub_call_dbproxy_handle.map_updata_persisted_object.Add(uuid_7864a402_2d75_5c02_b24b_50287a06732f, cb_updata_persisted_object_obj);
            }
            return cb_updata_persisted_object_obj;
        }

        public hub_call_dbproxy_find_and_modify_cb find_and_modify(string db, string collection, byte[] query_info, byte[] updata_info, bool _new, bool _upsert){
            var uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58 = new ArrayList();
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(db);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(collection);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(query_info);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(updata_info);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(_new);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(_upsert);
            call_module_method("hub_call_dbproxy_find_and_modify", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58);

            var cb_find_and_modify_obj = new hub_call_dbproxy_find_and_modify_cb(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_find_and_modify)
            {
                rsp_cb_hub_call_dbproxy_handle.map_find_and_modify.Add(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a, cb_find_and_modify_obj);
            }
            return cb_find_and_modify_obj;
        }

        public hub_call_dbproxy_remove_object_cb remove_object(string db, string collection, byte[] query_info){
            var uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = new ArrayList();
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f);
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(db);
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(collection);
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(query_info);
            call_module_method("hub_call_dbproxy_remove_object", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);

            var cb_remove_object_obj = new hub_call_dbproxy_remove_object_cb(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_remove_object)
            {
                rsp_cb_hub_call_dbproxy_handle.map_remove_object.Add(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, cb_remove_object_obj);
            }
            return cb_remove_object_obj;
        }

        public void get_object_info(string db, string collection, byte[] query_info, Int32 _skip, Int32 _limit, string _sort, bool _Ascending_, string callbackid){
            var _argv_1f17e6de_d423_391b_a599_7268e665a53f = new ArrayList();
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(db);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(collection);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(query_info);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(_skip);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(_limit);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(_sort);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(_Ascending_);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(callbackid);
            call_module_method("hub_call_dbproxy_get_object_info", _argv_1f17e6de_d423_391b_a599_7268e665a53f);
        }

        public hub_call_dbproxy_get_object_count_cb get_object_count(string db, string collection, byte[] query_info){
            var uuid_975425f5_8baf_5905_beeb_4454e78907f6 = (UInt32)Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);

            var _argv_2632cded_162c_3a9b_86ee_462b614cbeea = new ArrayList();
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(uuid_975425f5_8baf_5905_beeb_4454e78907f6);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(db);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(collection);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(query_info);
            call_module_method("hub_call_dbproxy_get_object_count", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);

            var cb_get_object_count_obj = new hub_call_dbproxy_get_object_count_cb(uuid_975425f5_8baf_5905_beeb_4454e78907f6, rsp_cb_hub_call_dbproxy_handle);
            lock(rsp_cb_hub_call_dbproxy_handle.map_get_object_count)
            {
                rsp_cb_hub_call_dbproxy_handle.map_get_object_count.Add(uuid_975425f5_8baf_5905_beeb_4454e78907f6, cb_get_object_count_obj);
            }
            return cb_get_object_count_obj;
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class dbproxy_call_hub_rsp_cb : abelkhan.Imodule {
        public dbproxy_call_hub_rsp_cb(abelkhan.modulemng modules) : base("dbproxy_call_hub_rsp_cb")
        {
        }

    }

    public class dbproxy_call_hub_caller : abelkhan.Icaller {
        public static dbproxy_call_hub_rsp_cb rsp_cb_dbproxy_call_hub_handle = null;
        private Int32 uuid_7a1d0ce9_a121_3019_b67a_319998ea37c8 = (Int32)RandomUUID.random();

        public dbproxy_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("dbproxy_call_hub", _ch)
        {
            if (rsp_cb_dbproxy_call_hub_handle == null)
            {
                rsp_cb_dbproxy_call_hub_handle = new dbproxy_call_hub_rsp_cb(modules);
            }
        }

        public void ack_get_object_info(string callbackid, byte[] object_info){
            var _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7 = new ArrayList();
            _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.Add(callbackid);
            _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.Add(object_info);
            call_module_method("dbproxy_call_hub_ack_get_object_info", _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7);
        }

        public void ack_get_object_info_end(string callbackid){
            var _argv_e4756ccf_94e2_3b4f_958a_701f7076e607 = new ArrayList();
            _argv_e4756ccf_94e2_3b4f_958a_701f7076e607.Add(callbackid);
            call_module_method("dbproxy_call_hub_ack_get_object_info_end", _argv_e4756ccf_94e2_3b4f_958a_701f7076e607);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class hub_call_dbproxy_reg_hub_rsp : abelkhan.Response {
        private UInt64 uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
        public hub_call_dbproxy_reg_hub_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
        }

        public void rsp(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("hub_call_dbproxy_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

        public void err(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("hub_call_dbproxy_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

    }

    public class hub_call_dbproxy_get_guid_rsp : abelkhan.Response {
        private UInt64 uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948;
        public hub_call_dbproxy_get_guid_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948 = _uuid;
        }

        public void rsp(Int64 guid_25cce41a_8adf_3dfe_9b75_9eb957e9743c){
            var _argv_8b362c4a_74a5_366e_a6af_37474d7fa521 = new ArrayList();
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948);
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(guid_25cce41a_8adf_3dfe_9b75_9eb957e9743c);
            call_module_method("hub_call_dbproxy_rsp_cb_get_guid_rsp", _argv_8b362c4a_74a5_366e_a6af_37474d7fa521);
        }

        public void err(){
            var _argv_8b362c4a_74a5_366e_a6af_37474d7fa521 = new ArrayList();
            _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.Add(uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948);
            call_module_method("hub_call_dbproxy_rsp_cb_get_guid_err", _argv_8b362c4a_74a5_366e_a6af_37474d7fa521);
        }

    }

    public class hub_call_dbproxy_create_persisted_object_rsp : abelkhan.Response {
        private UInt64 uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b;
        public hub_call_dbproxy_create_persisted_object_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b = _uuid;
        }

        public void rsp(){
            var _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = new ArrayList();
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);
            call_module_method("hub_call_dbproxy_rsp_cb_create_persisted_object_rsp", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }

        public void err(){
            var _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = new ArrayList();
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);
            call_module_method("hub_call_dbproxy_rsp_cb_create_persisted_object_err", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }

    }

    public class hub_call_dbproxy_updata_persisted_object_rsp : abelkhan.Response {
        private UInt64 uuid_16267d40_cddc_312f_87c0_185a55b79ad2;
        public hub_call_dbproxy_updata_persisted_object_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_16267d40_cddc_312f_87c0_185a55b79ad2 = _uuid;
        }

        public void rsp(){
            var _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = new ArrayList();
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(uuid_16267d40_cddc_312f_87c0_185a55b79ad2);
            call_module_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_rsp", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }

        public void err(){
            var _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = new ArrayList();
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(uuid_16267d40_cddc_312f_87c0_185a55b79ad2);
            call_module_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_err", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }

    }

    public class hub_call_dbproxy_find_and_modify_rsp : abelkhan.Response {
        private UInt64 uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae;
        public hub_call_dbproxy_find_and_modify_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae = _uuid;
        }

        public void rsp(byte[] object_info_95c5b6a3_d4a6_3baf_8fea_9e22167f3d40){
            var _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58 = new ArrayList();
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(object_info_95c5b6a3_d4a6_3baf_8fea_9e22167f3d40);
            call_module_method("hub_call_dbproxy_rsp_cb_find_and_modify_rsp", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58);
        }

        public void err(){
            var _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58 = new ArrayList();
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.Add(uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae);
            call_module_method("hub_call_dbproxy_rsp_cb_find_and_modify_err", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58);
        }

    }

    public class hub_call_dbproxy_remove_object_rsp : abelkhan.Response {
        private UInt64 uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1;
        public hub_call_dbproxy_remove_object_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1 = _uuid;
        }

        public void rsp(){
            var _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = new ArrayList();
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);
            call_module_method("hub_call_dbproxy_rsp_cb_remove_object_rsp", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }

        public void err(){
            var _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = new ArrayList();
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);
            call_module_method("hub_call_dbproxy_rsp_cb_remove_object_err", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }

    }

    public class hub_call_dbproxy_get_object_count_rsp : abelkhan.Response {
        private UInt64 uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2;
        public hub_call_dbproxy_get_object_count_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2 = _uuid;
        }

        public void rsp(UInt32 count_5292aa87_6be8_3979_a905_592fb57973b5){
            var _argv_2632cded_162c_3a9b_86ee_462b614cbeea = new ArrayList();
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(count_5292aa87_6be8_3979_a905_592fb57973b5);
            call_module_method("hub_call_dbproxy_rsp_cb_get_object_count_rsp", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }

        public void err(){
            var _argv_2632cded_162c_3a9b_86ee_462b614cbeea = new ArrayList();
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2);
            call_module_method("hub_call_dbproxy_rsp_cb_get_object_count_err", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }

    }

    public class hub_call_dbproxy_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_dbproxy_module(abelkhan.modulemng _modules) : base("hub_call_dbproxy")
        {
            modules = _modules;
            modules.reg_method("hub_call_dbproxy_reg_hub", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, reg_hub));
            modules.reg_method("hub_call_dbproxy_get_guid", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_guid));
            modules.reg_method("hub_call_dbproxy_create_persisted_object", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, create_persisted_object));
            modules.reg_method("hub_call_dbproxy_updata_persisted_object", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, updata_persisted_object));
            modules.reg_method("hub_call_dbproxy_find_and_modify", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, find_and_modify));
            modules.reg_method("hub_call_dbproxy_remove_object", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, remove_object));
            modules.reg_method("hub_call_dbproxy_get_object_info", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_object_info));
            modules.reg_method("hub_call_dbproxy_get_object_count", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, get_object_count));
        }

        public event Action<string> on_reg_hub;
        public void reg_hub(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _hub_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            rsp.Value = new hub_call_dbproxy_reg_hub_rsp(current_ch.Value, _cb_uuid);
            if (on_reg_hub != null){
                on_reg_hub(_hub_name);
            }
            rsp.Value = null;
        }

        public event Action<string, string, string> on_get_guid;
        public void get_guid(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _db = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _guid_key = ((MsgPack.MessagePackObject)inArray[3]).AsString();
            rsp.Value = new hub_call_dbproxy_get_guid_rsp(current_ch.Value, _cb_uuid);
            if (on_get_guid != null){
                on_get_guid(_db, _collection, _guid_key);
            }
            rsp.Value = null;
        }

        public event Action<string, string, byte[]> on_create_persisted_object;
        public void create_persisted_object(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _db = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _object_info = ((MsgPack.MessagePackObject)inArray[3]).AsBinary();
            rsp.Value = new hub_call_dbproxy_create_persisted_object_rsp(current_ch.Value, _cb_uuid);
            if (on_create_persisted_object != null){
                on_create_persisted_object(_db, _collection, _object_info);
            }
            rsp.Value = null;
        }

        public event Action<string, string, byte[], byte[], bool> on_updata_persisted_object;
        public void updata_persisted_object(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _db = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _query_info = ((MsgPack.MessagePackObject)inArray[3]).AsBinary();
            var _updata_info = ((MsgPack.MessagePackObject)inArray[4]).AsBinary();
            var __upsert = ((MsgPack.MessagePackObject)inArray[5]).AsBoolean();
            rsp.Value = new hub_call_dbproxy_updata_persisted_object_rsp(current_ch.Value, _cb_uuid);
            if (on_updata_persisted_object != null){
                on_updata_persisted_object(_db, _collection, _query_info, _updata_info, __upsert);
            }
            rsp.Value = null;
        }

        public event Action<string, string, byte[], byte[], bool, bool> on_find_and_modify;
        public void find_and_modify(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _db = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _query_info = ((MsgPack.MessagePackObject)inArray[3]).AsBinary();
            var _updata_info = ((MsgPack.MessagePackObject)inArray[4]).AsBinary();
            var __new = ((MsgPack.MessagePackObject)inArray[5]).AsBoolean();
            var __upsert = ((MsgPack.MessagePackObject)inArray[6]).AsBoolean();
            rsp.Value = new hub_call_dbproxy_find_and_modify_rsp(current_ch.Value, _cb_uuid);
            if (on_find_and_modify != null){
                on_find_and_modify(_db, _collection, _query_info, _updata_info, __new, __upsert);
            }
            rsp.Value = null;
        }

        public event Action<string, string, byte[]> on_remove_object;
        public void remove_object(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _db = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _query_info = ((MsgPack.MessagePackObject)inArray[3]).AsBinary();
            rsp.Value = new hub_call_dbproxy_remove_object_rsp(current_ch.Value, _cb_uuid);
            if (on_remove_object != null){
                on_remove_object(_db, _collection, _query_info);
            }
            rsp.Value = null;
        }

        public event Action<string, string, byte[], Int32, Int32, string, bool, string> on_get_object_info;
        public void get_object_info(IList<MsgPack.MessagePackObject> inArray){
            var _db = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _query_info = ((MsgPack.MessagePackObject)inArray[2]).AsBinary();
            var __skip = ((MsgPack.MessagePackObject)inArray[3]).AsInt32();
            var __limit = ((MsgPack.MessagePackObject)inArray[4]).AsInt32();
            var __sort = ((MsgPack.MessagePackObject)inArray[5]).AsString();
            var __Ascending_ = ((MsgPack.MessagePackObject)inArray[6]).AsBoolean();
            var _callbackid = ((MsgPack.MessagePackObject)inArray[7]).AsString();
            if (on_get_object_info != null){
                on_get_object_info(_db, _collection, _query_info, __skip, __limit, __sort, __Ascending_, _callbackid);
            }
        }

        public event Action<string, string, byte[]> on_get_object_count;
        public void get_object_count(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _db = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _collection = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            var _query_info = ((MsgPack.MessagePackObject)inArray[3]).AsBinary();
            rsp.Value = new hub_call_dbproxy_get_object_count_rsp(current_ch.Value, _cb_uuid);
            if (on_get_object_count != null){
                on_get_object_count(_db, _collection, _query_info);
            }
            rsp.Value = null;
        }

    }
    public class dbproxy_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public dbproxy_call_hub_module(abelkhan.modulemng _modules) : base("dbproxy_call_hub")
        {
            modules = _modules;
            modules.reg_method("dbproxy_call_hub_ack_get_object_info", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, ack_get_object_info));
            modules.reg_method("dbproxy_call_hub_ack_get_object_info_end", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, ack_get_object_info_end));
        }

        public event Action<string, byte[]> on_ack_get_object_info;
        public void ack_get_object_info(IList<MsgPack.MessagePackObject> inArray){
            var _callbackid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _object_info = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            if (on_ack_get_object_info != null){
                on_ack_get_object_info(_callbackid, _object_info);
            }
        }

        public event Action<string> on_ack_get_object_info_end;
        public void ack_get_object_info_end(IList<MsgPack.MessagePackObject> inArray){
            var _callbackid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_ack_get_object_info_end != null){
                on_ack_get_object_info_end(_callbackid);
            }
        }

    }

}
