using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace config
{
    public class config
	{
        public config(String file_path)
        {
			FileStream fs = File.OpenRead(file_path);
			byte[] data = new byte[fs.Length];
			int offset = 0;
			int remaining = data.Length;
			while (remaining > 0)
			{

				int read = fs.Read(data, offset, remaining);
				if (read <= 0)
					throw new EndOfStreamException("file read at" + read.ToString() + " failed");

				remaining -= read;
				offset += read;
			}

			handle = System.Text.Json.Jsonparser.unpack(System.Text.Encoding.Default.GetString(data));
        }

		private config(object sub_handle)
		{
			handle = sub_handle;
		}

		public bool has_key(String key)
		{
			return ((Hashtable)handle).ContainsKey(key);
		}

		public bool get_value_bool(String key)
		{
			return (bool)((Hashtable)handle)[key];
		}

		public Int64 get_value_int(String key)
		{
			return (Int64)((Hashtable)handle)[key];
		}

		public double get_value_float(String key)
		{
			return (double)((Hashtable)handle)[key];
		}

		public String get_value_string(String key)
		{
			return (String)((Hashtable)handle)[key];
		}

		public config get_value_dict(String key)
		{
			var _handle = ((Hashtable)handle)[key];

			return new config(_handle);
		}

		public config get_value_list(String key)
		{
			var _handle = ((Hashtable)handle)[key];

			return new config(_handle);
		}

		public Int64 get_list_size()
		{
			return ((ArrayList)handle).Count;
		}

		public bool get_list_bool(int index)
		{
			return (bool)(((ArrayList)handle)[index]);
		}

		public Int64 get_list_int(int index)
		{
			return (Int64)(((ArrayList)handle)[index]);
		}

		public double get_list_float(int index)
		{
			return (double)(((ArrayList)handle)[index]);
		}

		public String get_list_string(int index)
		{
			return (String)(((ArrayList)handle)[index]);
		}

		public config get_list_dict(int index)
		{
			var _handle = (((ArrayList)handle)[index]);

			return new config(_handle);
		}

		private object handle;
    }
}
