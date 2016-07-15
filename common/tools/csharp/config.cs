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

		bool get_value_bool(String key)
		{
			return (bool)((Hashtable)handle)[key];
		}

		Int64 get_value_int(String key)
		{
			return (Int64)((Hashtable)handle)[key];
		}

		double get_value_float(String key)
		{
			return (double)((Hashtable)handle)[key];
		}

		String get_value_string(String key)
		{
			return (String)((Hashtable)handle)[key];
		}

		config get_value_dict(String key)
		{
			var _handle = ((Hashtable)handle)[key];

			return new config(_handle);
		}

		config get_value_list(String key)
		{
			var _handle = ((Hashtable)handle)[key];

			return new config(_handle);
		}

		Int64 get_list_size()
		{
			return ((ArrayList)handle).Count;
		}

		bool get_list_bool(int index)
		{
			return (bool)(((ArrayList)handle)[index]);
		}

		Int64 get_list_int(int index)
		{
			return (Int64)(((ArrayList)handle)[index]);
		}

		double get_list_float(int index)
		{
			return (double)(((ArrayList)handle)[index]);
		}

		String get_list_string(int index)
		{
			return (String)(((ArrayList)handle)[index]);
		}

		config get_list_dict(int index)
		{
			var _handle = (((ArrayList)handle)[index]);

			return new config(_handle);
		}

		private object handle;
    }
}
