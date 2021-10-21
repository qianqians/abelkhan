using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MsgPack.Serialization;

namespace abelkhan
{
    public class Exception : System.Exception
    {
        public Exception(string _err) : base(_err)
        {
        }
    }

    public class RandomUUID
    {
        private static Random ran = new Random();
        public static UInt64 random()
        {
            return (UInt64)(ran.NextDouble() * UInt64.MaxValue);
        }
    }

    public class TinyTimer
    {
        private static UInt64 tick;
        private static Dictionary<UInt64, Action> timer = new Dictionary<UInt64, Action>();

        private static UInt64 refresh()
        {
            return (UInt64)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static void add_timer(UInt64 _tick, Action cb)
        {
            tick = refresh();
            var tick_ = tick + _tick;
            while(timer.ContainsKey(tick_)){ tick_++; }

            lock(timer)
            {
                timer.Add(tick_, cb);
            }
        }
        public static void poll()
        {
            tick = refresh();

            lock(timer)
            {
                var list = new List<UInt64>();
                foreach (var item in timer)
				{
					if (item.Key <= tick)
					{
						list.Add(item.Key);
                        item.Value();
					}
                    else
                    {
                        break;
                    }
				}
                foreach (var item in list)
				{
					timer.Remove(item);
				}
            }
        }
    }

    public interface Ichannel
    {
        ArrayList pop();
        void disconnect();
        void send(byte[] data);
    }

    public class Icaller
    {
        public Icaller(String _module_name, Ichannel _ch)
        {
            module_name = _module_name;
            ch = _ch;
        }

        public void call_module_method(String methodname, ArrayList argvs)
        {
			ArrayList _event = new ArrayList();
            _event.Add(module_name);
            _event.Add(methodname);
            _event.Add(argvs);

            try
            {
                using (MemoryStream stream = new MemoryStream(), send_st = new MemoryStream())
                {
                    var serializer = MessagePackSerializer.Get<ArrayList>();
                    serializer.Pack(stream, _event);
                    stream.Position = 0;
                    var data = stream.ToArray();

                    var _tmplenght = data.Length;
                    send_st.WriteByte((byte)(_tmplenght & 0xff));
                    send_st.WriteByte((byte)((_tmplenght >> 8) & 0xff));
                    send_st.WriteByte((byte)((_tmplenght >> 16) & 0xff));
                    send_st.WriteByte((byte)((_tmplenght >> 24) & 0xff));
                    send_st.Write(data, 0, _tmplenght);
                    send_st.Position = 0;

                    ch.send(send_st.ToArray());
                }
            }
            catch (System.Exception)
            {
                throw new abelkhan.Exception("error argvs");
            }
        }

        protected String module_name;
        private Ichannel ch;
    }

    public class Response : Icaller{
        public Response(String _module_name, Ichannel _ch) : base(_module_name, _ch){ 
        }
    }

    public class Imodule
    {
        protected Dictionary<string, Action<IList<MsgPack.MessagePackObject> > > events;

        public Imodule(String _module_name){
            module_name = _module_name;
            events = new Dictionary<string, Action<IList<MsgPack.MessagePackObject> > >();
            current_ch = null;
            rsp = null;
        }

        public void reg_method(String method_name, Action<IList<MsgPack.MessagePackObject> > method){
            events.Add(method_name, method);
        }

        public void process_event(Ichannel _ch, ArrayList _event)
		{
			current_ch = _ch;
            try
            {
                String func_name = ((MsgPack.MessagePackObject)_event[1]).AsString();

                if (events.TryGetValue(func_name, out Action<IList<MsgPack.MessagePackObject> > method))
                {
                    try
                    {
                        method(((MsgPack.MessagePackObject)_event[2]).AsList());
                    }
                    catch (System.Exception e)
                    {
                        throw new abelkhan.Exception(string.Format("function name:{0} System.Exception:{1}", func_name, e));
                    }
                }
                else
                {
                    throw new abelkhan.Exception(string.Format("do not have a function named::{0}", func_name));
                }
            }
            catch (System.Exception e)
            {
                throw new abelkhan.Exception(string.Format("System.Exception:{0}", e));
            }
            finally
            {
                current_ch = null;
            }
        }

		public Ichannel current_ch;
        public Response rsp;
		public String module_name;
    }

    public class modulemng
    {
		public modulemng()
		{
			module_set = new Dictionary<string, Imodule>();
		}

		public void reg_module(Imodule module)
        {
			module_set.Add(module.module_name, module);
        }

		public void unreg_module(Imodule module)
        {
			module_set.Remove(module.module_name);
        }

        public void process_event(Ichannel _ch, ArrayList _event){
            try{
                String module_name = ((MsgPack.MessagePackObject)_event[0]).AsString();
                if (module_set.TryGetValue(module_name, out Imodule _module))
                {
                    _module.process_event(_ch, _event);
                }
                else
                {
                    throw new abelkhan.Exception(string.Format("do not have a module named::{0}", module_name));
                }
            }
            catch (System.Exception e)
            {
                throw new abelkhan.Exception(string.Format("System.Exception:{0}", e));
            }
        }

        private Dictionary<string, Imodule> module_set;
    }
}