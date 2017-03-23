using System;
using System.Collections;

namespace json_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "{\"key\":\"value\"}";
            Hashtable o = (Hashtable)System.Text.Json.Jsonparser.unpack(str);

            Hashtable t = new Hashtable();
            t.Add("key1", 1);
            t.Add("key2", "2");
            t.Add("key3", 3.0);
            t.Add("key4", true);
            string s = System.Text.Json.Jsonparser.pack(t);
        }
    }
}
