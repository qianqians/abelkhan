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
            t.Add("key3", 3.1);
            t.Add("key4", true);
            string s = System.Text.Json.Jsonparser.pack(t);
            o = (Hashtable)System.Text.Json.Jsonparser.unpack(s);

            Hashtable t1 = new Hashtable();
            t1.Add("key1", 1);
            t1.Add("key2", "2");
            t1.Add("key3", 3.2);
            t1.Add("key4", true);
            t1.Add("key5", t);
            s = System.Text.Json.Jsonparser.pack(t1);
            o = (Hashtable)System.Text.Json.Jsonparser.unpack(s);

            ArrayList l = new ArrayList();
            l.Add("v1");
            l.Add(1);
            l.Add(2.2);
            l.Add(false);
            t1.Add("key6", l);
            s = System.Text.Json.Jsonparser.pack(t1);
            o = (Hashtable)System.Text.Json.Jsonparser.unpack(s);

            ArrayList l1 = new ArrayList();
            l1.Add("v1");
            l1.Add(1);
            l1.Add(2.2);
            l1.Add(false);
            l1.Add(l);
            t1.Add("key7", l1);
            s = System.Text.Json.Jsonparser.pack(t1);
            o = (Hashtable)System.Text.Json.Jsonparser.unpack(s);
        }
    }
}
