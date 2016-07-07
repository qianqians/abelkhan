using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace juggle
{
    public class Icaller
    {
        public Icaller(Ichannel _ch)
        {
            ch = _ch;
        }

        public void call_module_method(String methodname, ArrayList argvs)
        {
			ArrayList _event = new ArrayList();
            _event.Add(module_name);
            _event.Add(methodname);
            _event.Add(argvs);

			var _tmp = System.Text.Json.Jsonparser.pack(_event);

			byte[] buf = new byte[4 + _tmp.Length];
			buf[0] = (byte)(_tmp.Length & 0xff);
			buf[1] = (byte)((_tmp.Length >> 8) & 0xff);
			buf[1] = (byte)((_tmp.Length >> 16) & 0xff);
			buf[1] = (byte)((_tmp.Length >> 24) & 0xff);
			//BitConverter.GetBytes(_tmp.Length).CopyTo(buf, 0);
			System.Text.Encoding.Default.GetBytes(_tmp).CopyTo(buf, 4);

			ch.senddata(buf);
        }

        protected String module_name;
        private Ichannel ch;
    }
}
