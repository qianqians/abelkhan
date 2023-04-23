﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace common
{
	public class Modulemanager
	{
		public Modulemanager()
		{
			motheds = new Dictionary<string, Action<IList<MsgPack.MessagePackObject> > >();
		}

		public void add_mothed(string cb_name, Action<IList<MsgPack.MessagePackObject>> cb)
		{
			motheds.Add(cb_name, cb);
		}

		public void process_module_mothed(String func_name, IList<MsgPack.MessagePackObject> argvs)
		{
            if (motheds.TryGetValue(func_name, out Action<IList<MsgPack.MessagePackObject> > mothed))
			{
				mothed.Invoke(argvs);
			}
			else
            {
                log.Log.err("do not have a mothed name:{0}", func_name);
				throw new moduleException(String.Format("modulemanager.process_module_mothed unreg mothed name:{0}!", func_name));
			}
		}

		private Dictionary<string, Action<IList<MsgPack.MessagePackObject> > > motheds;
	}
}

