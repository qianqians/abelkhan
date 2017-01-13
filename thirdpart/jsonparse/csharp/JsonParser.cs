using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace System.Text.Json
{
    class Jsonparser
    {
        public static String pack(object dict)
        {
            Func<object, String> parsevalue = (object value) =>
            {
                String _out = "";
                if (typeof(String).IsInstanceOfType(value))
                {
                    _out += "\"";
                }
                _out += Convert.ToString(value);
                if (typeof(String).IsInstanceOfType(value))
                {
                    _out += "\"";
                }
                return _out;
            };

            Func<ArrayList, String> parselist = (ArrayList _array) =>
            {
                String _out = "[";
                foreach (Object o in _array)
                {
                    if ((typeof(Hashtable).IsInstanceOfType(o)) || (typeof(ArrayList).IsInstanceOfType(o)))
                    {
                        _out += pack(o);
                    }
                    else
                    {
                        _out += parsevalue(o);
                    }
                    _out += ",";
                }
                _out = _out.Remove(_out.Length - 1, 1);
                _out += "]";

                return _out;
            };

            Func<Hashtable, String> parsedict = (Hashtable _dict) =>{
                String _out = "{";
                foreach (System.Collections.DictionaryEntry _obj in _dict)
                {
                    _out += "\"" + Convert.ToString(_obj.Key) + "\"";
                    _out += ":";
                    if ((typeof(Hashtable).IsInstanceOfType(_obj)) || (typeof(Array).IsInstanceOfType(_obj)))
                    {
                        _out += pack(_obj.Value);
                    }
                    else
                    {
                        _out += parsevalue(_obj.Value);
                    }
                    _out += ",";
                }
                _out = _out.Remove(_out.Length - 1, 1);
                _out += "}";

                return _out;
            };

            Func<object, String> parse = (object o) =>
            {
                if (typeof(Hashtable).IsInstanceOfType(o))
                {
                    return parsedict((Hashtable)o);
                }
                else if (typeof(ArrayList).IsInstanceOfType(o))
                {
                    return parselist((ArrayList)o);
                }
				else
				{
                    throw new System.Exception("can not parse this object to json");
				}
            };

            return parse(dict);
        }

        public static object unpack(String jsonstr)
        {
            object _out = null;
            Hashtable _table = null;
            ArrayList _array = null;

            String key = "";
            String value = "";
            CharEnumerator c = jsonstr.GetEnumerator();

            Stack s = new Stack();

            Func<String, String> parsekey = (String _c) =>{
                key += _c;
                return key;
            };

            Func<String, String> parsevalue = (String _c) =>
            {
                value += _c;
                return value;
            };

            Func<String, String> parsesys = parsekey;

            Func<String, String> parsemap = (String _c) =>
            {
                parsesys = parsekey;

                if (value == "" || key == "")
                {
                    return _c;
                }

                value = value.Trim();
                while (value[0] == '\n' || value[0] == '\t' || value[0] == '\0')
                {
                    value = value.Substring(1, value.Length - 1);
                }
                String v = value;
                key = key.Trim();
                while (key[0] == '\n' || key[0] == '\t' || key[0] == '\0')
                {
                    key = key.Substring(1, key.Length - 1);
                }
                key = key.Substring(1, key.Length - 2);

                if (v == "true")
                {
                    _table[key] = true;
                }
                else if (v == "false")
                {
                    _table[key] = false;
                }
                else if (v == "null")
                {
                    _table[key] = null;
                }
                else
                {
                    if (v[0] == '\"' && v[v.Length - 1] == '\"')
                    {
                        v = v.Substring(1, v.Length - 2);
                        _table[key] = v;
                    }
                    else
                    {
                        int status = 0;

                        foreach (char _ch in v)
                        {
                            if ((_ch < '0' || _ch > '9') && _ch != '.')
                            {
                                throw new Exception("format error");
                            }

                            if (_ch == '.')
                            {
                                status++;
                            }
                        }

                        if (status == 0)
                        {
                            _table[key] = Convert.ToInt64(v);
                        }
                        else if (status == 1)
                        {
                            _table[key] = Convert.ToDouble(v);
                        }
                        else
                        {
                            throw new Exception("format error");
                        }
                    }
                    
                }

                key = "";
                value = "";

                return _c;
            };

            Func<String, String> parsearray = (String _c) =>
            {
                value = value.Trim();

                if (value == "")
                {
                    return _c;
                }

                while (value[0] == '\n' || value[0] == '\t' || value[0] == '\0')
                {
                    value = value.Substring(1, value.Length - 1);
                }
                String v = value;

                if (v.ToLower() == "true")
                {
                    _array.Add(true);
                }
                else if (v.ToLower() == "false")
                {
                    _array.Add(false);
                }
                else if (v.ToLower() == "null")
                {
                    _array.Add(null);
                }
                else
                {
                    if (v[0] == '\"' && v[v.Length - 1] == '\"')
                    {
                        v = v.Substring(1, v.Length - 2);
                        _array.Add(v);
                    }
                    else
                    {
                        int status = 0;

                        foreach (char _ch in v)
                        {
                            if ((_ch < '0' || _ch > '9') && _ch != '.')
                            {
                                throw new Exception("format error");
                            }

                            if (_ch == '.')
                            {
                                status++;
                            }
                        }

                        if (status == 0)
                        {
                            _array.Add(Convert.ToInt64(v));
                        }
                        else if (status == 1)
                        {
                            _array.Add(Convert.ToDouble(v));
                        }
                        else
                        {
                            throw new Exception("format error");
                        }
                    }

                }

                key = "";
                value = "";

                return _c;
            };

            Func<String, String> parseenum = parsemap;

            Func<object, object> parsepop = (object o) =>{
                if (typeof(Hashtable).IsInstanceOfType(o))
                {
                    _table = (Hashtable)o;

                    parsesys = parsekey;
                    parseenum = parsemap;
                }
                else if (typeof(ArrayList).IsInstanceOfType(o))
                {
                    _array = (ArrayList)o;

                    parsesys = parsevalue;
                    parseenum = parsearray;
                }

                return o;
            };

            int count = 0;
            int escape = 0;
            while (c.MoveNext())
            {
                if (c.Current.ToString() == "\\"){
                    escape = 1;
                }else{
                    escape = 0;
                }

                if (c.Current.ToString() == "\"" && escape != 1)
                {
                    if (count == 0)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }
                }

                if (count == 0)
                {
                    if (c.Current.ToString() == "{")
                    {
                        parsesys = parsekey;
                        parseenum = parsemap;

                        Hashtable _newtable = new Hashtable();
                        if (_table != null)
                        {
                            key = key.Trim();
                            while (key[0] == '\n' || key[0] == '\t' || key[0] == '\0')
                            {
                                key = key.Substring(1, key.Length - 1);
                            }
                            s.Push(_table);
                            key = key.Substring(1, key.Length - 2);
                            _table[key] = _newtable;
                            _table = null;
                            key = "";
                        }
                        else if (_array != null)
                        {
                            s.Push(_array);
                            _array.Add(_newtable);
                            _array = null;
                        }
                        _table = _newtable;

                        continue;
                    }

                    if (c.Current.ToString() == "}")
                    {
                        parseenum(c.Current.ToString());

                        if (s.Count > 0)
                        {
                            parsepop(s.Pop());
                        }

                        continue;
                    }

                    if (c.Current.ToString() == "[")
                    {
                        parsesys = parsevalue;
                        parseenum = parsearray;

                        ArrayList _newarray = new ArrayList();
                        if (_table != null)
                        {
                            s.Push(_table);
                            key = key.Trim();
                            while (key[0] == '\n' || key[0] == '\t' || key[0] == '\0')
                            {
                                key = key.Substring(1, key.Length - 1);
                            }
                            key = key.Substring(1, key.Length - 2);
                            _table[key] = _newarray;
                            _table = null;
                            key = "";
                        }
                        else if (_array != null)
                        {
                            s.Push(_array);
                            _array.Add(_newarray);
                            _array = null;
                        }
                        _array = _newarray;

                        continue;
                    }

                    if (c.Current.ToString() == "]")
                    {
                        parseenum(c.Current.ToString());

						if (s.Count > 0)
						{
							parsepop(s.Pop());
						}

                        continue;
                    }

                    if (c.Current.ToString() == ",")
                    {
                        parseenum(c.Current.ToString());
                        continue;
                    }

                    if (c.Current.ToString() == ":")
                    {
                        parsesys = parsevalue;
                        continue;
                    }
                }

                parsesys(c.Current.ToString());

            }

            if (_table != null)
            {
                _out = _table;
            }
            else if (_array != null)
            {
                _out = _array;
            }

            return _out;
        }
    }
}
