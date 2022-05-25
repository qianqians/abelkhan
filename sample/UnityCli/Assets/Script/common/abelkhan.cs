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

		public Ichannel current_ch;
        public Response rsp;
		public String module_name;
    }

    public class modulemng
    {
		public modulemng()
		{
			method_set = new Dictionary<string, Tuple<Imodule, Action<IList<MsgPack.MessagePackObject> > > >();
		}

        public void reg_method(String method_name, Tuple<Imodule, Action<IList<MsgPack.MessagePackObject> > > method){
            method_set.Add(method_name, method);
        }

        public Action<abelkhan.Ichannel> on_msg;
        public void process_event(Ichannel _ch, ArrayList _event){
            try{
                String method_name = ((MsgPack.MessagePackObject)_event[0]).AsString();
                if (method_set.TryGetValue(method_name, out Tuple<Imodule, Action<IList<MsgPack.MessagePackObject> > > _method))
                {
                    _method.Item1.current_ch = _ch;
                    _method.Item2.Invoke(((MsgPack.MessagePackObject)_event[1]).AsList());
                    on_msg?.Invoke(_ch);
                    _method.Item1.current_ch = null;
                }
                else
                {
                    throw new abelkhan.Exception(string.Format("do not have a method named::{0}", method_name));
                }
            }
            catch (System.Exception e)
            {
                throw new abelkhan.Exception(string.Format("System.Exception:{0}", e));
            }
        }

        private Dictionary<string, Tuple<Imodule, Action<IList<MsgPack.MessagePackObject> > > > method_set;
    }
}