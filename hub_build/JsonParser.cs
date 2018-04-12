using System;
using System.Text;
using System.Collections;

namespace Json
{
    public class Exception : System.Exception
    {
        public Exception(string err) : base(err)
        {
        }
    }

    public class Jsonparser
    {
        public static String pack(object dict)
        {
            Func<String, String> preprocess = (String str) =>
            {
                StringBuilder _out = new StringBuilder();

                CharEnumerator c = str.GetEnumerator();
                while (c.MoveNext()) 
                {
                    switch(c.Current.ToString())
                    {
                        case "\"":
                            _out.Append("\\\"");
                            break;

                        default:
                            _out.Append(c.Current.ToString());
                            break;
                    }


                } 

                return _out.ToString();
            };

            Func<String, String> parsestring = (String str) =>
            {
                StringBuilder _out = new StringBuilder();
                _out.AppendFormat("\"{0}\"", preprocess(str));

                return _out.ToString();
            };

            Func<object, String> parsevalue = (object value) =>
            {
                StringBuilder _out = new StringBuilder();
                if (typeof(String).IsInstanceOfType(value))
                {
                    _out.Append(parsestring((String)value));
                }
                else if (typeof(bool).IsInstanceOfType(value))
                {
                    _out.Append(Convert.ToString(value).ToLower());
                }
                else if (value == null)
                {
                    _out.Append("null");
                }
                else
                {
                    _out.Append(Convert.ToString(value));
                }

                return _out.ToString();
            };

            Func<ArrayList, String> parselist = (ArrayList _array) =>
            {
                StringBuilder _out = new StringBuilder("[");
                if (_array.Count > 0)
                {
                    foreach (Object o in _array)
                    {
                        if ((typeof(Hashtable).IsInstanceOfType(o)) || (typeof(ArrayList).IsInstanceOfType(o)) || (typeof(Array).IsInstanceOfType(o)))
                        {
                            _out.Append(pack(o));
                        }
                        else
                        {
                            _out.Append(parsevalue(o));
                        }
                        _out.Append(",");
                    }
                    _out.Remove(_out.Length - 1, 1);
                }
                _out.Append("]");

                return _out.ToString();
            };

            Func<Array, String> parsearray = (Array _array) =>
            {
                StringBuilder _out = new StringBuilder("[");
                if (_array.Length > 0)
                {
                    foreach (Object o in _array)
                    {
                        if ((typeof(Hashtable).IsInstanceOfType(o)) || (typeof(ArrayList).IsInstanceOfType(o)) || (typeof(Array).IsInstanceOfType(o)))
                        {
                            _out.Append(pack(o));
                        }
                        else
                        {
                            _out.Append(parsevalue(o));
                        }
                        _out.Append(",");
                    }
                    _out.Remove(_out.Length - 1, 1);
                }
                _out.Append("]");

                return _out.ToString();
            };

            Func<Hashtable, String> parsedict = (Hashtable _dict) =>{
                StringBuilder _out = new StringBuilder("{"); 
                if (_dict.Count > 0)
                {
                    foreach (System.Collections.DictionaryEntry _obj in _dict)
                    {
                        _out.Append(parsevalue(_obj.Key));
                        _out.Append(":");
                        if ((typeof(Hashtable).IsInstanceOfType(_obj.Value)) || (typeof(ArrayList).IsInstanceOfType(_obj.Value)) || (typeof(Array).IsInstanceOfType(_obj.Value)))
                        {
                            _out.Append(pack(_obj.Value));
                        }
                        else
                        {
                            _out.Append(parsevalue(_obj.Value));
                        }
                        _out.Append(",");
                    }
                    _out.Remove(_out.Length - 1, 1);
                }
                _out.Append("}");

                return _out.ToString();
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
                else if (typeof(Array).IsInstanceOfType(o))
                {
                    return parsearray((Array)o);
                }
				else
				{
                    throw new Json.Exception("can not parse this object to json");
				}
            };

            return parse(dict);
        }

        public static object unpack(String jsonstr)
        {
            object _out = null;
            Hashtable _table = null;
            ArrayList _array = null;

            StringBuilder key = new StringBuilder();
            StringBuilder value = new StringBuilder();
            CharEnumerator c = jsonstr.GetEnumerator();

            Stack s = new Stack();

            Func<String, String> afterprocess = (String str) =>
            {
                Func<String, String> escape_ = (String _char) =>
                {
                    String char_ = "";

                    switch(_char)
                    {
                        default:
                            char_ = _char;
                            break;
                    }

                    return char_;
                };

                StringBuilder out_ = new StringBuilder();

                CharEnumerator _c = str.GetEnumerator();
                while (_c.MoveNext()) 
                {
                    switch (_c.Current.ToString())
                    {
                        case "\\":
                            _c.MoveNext();
                            out_.Append(escape_(_c.Current.ToString()));
                            break;
                        default:
                            out_.Append(_c.Current.ToString());
                            break;
                    }

                } 

                return out_.ToString();
            };

            Action<String> parsekey = (String _c) =>
            {
                key.Append(_c);
            };

            Action<String> parsevalue = (String _c) =>
            {
                value.Append(_c);
            };

            Action<String> parsesys = parsekey;

            Func<String, String> parsemap = (String _c) =>
            {
                parsesys = parsekey;

                if (value.ToString() == "" || key.ToString() == "")
                {
                    return _c;
                }

                string _value = value.ToString().Trim();
                while (_value[0] == '\n' || _value[0] == '\t' || _value[0] == '\0')
                {
                    _value = _value.Substring(1, _value.Length - 1);
                }
                String v = _value;

                string _key = key.ToString().Trim();
                while (_key[0] == '\n' || _key[0] == '\t' || _key[0] == '\0')
                {
                    _key = _key.Substring(1, _key.Length - 1);
                }
                _key = _key.Substring(1, _key.Length - 2);
                _key = afterprocess(_key);

                if (v == "true")
                {
                    _table[_key] = true;
                }
                else if (v == "false")
                {
                    _table[_key] = false;
                }
                else if (v == "null")
                {
                    _table[_key] = null;
                }
                else
                {
                    if (v[0] == '\"' && v[v.Length - 1] == '\"')
                    {
                        v = v.Substring(1, v.Length - 2);
                        _table[_key] = afterprocess(v);
                    }
                    else
                    {
                        int status = 0;
                        int E_status = 0;

                        for (int i = 0; i < v.Length; i++)
                        {
                            if (v[i] == '-')
                            {
                                if (i == 0 || v[i - 1] == 'E')
                                {
                                    continue;
                                }
                                else
                                {
                                    throw new Json.Exception("format error");
                                }
                            }

                            if ((v[i] < '0' || v[i] > '9') && v[i] != '.' && v[i] != 'E')
                            {
                                throw new Json.Exception("format error");
                            }

                            if (v[i] == '.')
                            {
                                status++;
                            }

                            if (v[i] == 'E')
                            {
                                E_status++;
                            }
                        }

                        if (status == 0 && E_status == 0)
                        {
                            _table[_key] = Convert.ToInt64(v);
                        }
                        else if (status == 1 && (E_status == 0 || E_status == 1))
                        {
                            _table[_key] = Convert.ToDouble(v);
                        }
                        else
                        {
                            throw new Json.Exception("format error");
                        }
                    }
                    
                }

                key = new StringBuilder();
                value = new StringBuilder();

                return _c;
            };

            Func<String, String> parsearray = (String _c) =>
            {
                string _value = value.ToString().Trim();

                if (_value == "")
                {
                    return _c;
                }

                while (_value[0] == '\n' || _value[0] == '\t' || _value[0] == '\0')
                {
                    _value = _value.Substring(1, _value.Length - 1);
                }
                String v = _value;

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
                        _array.Add(afterprocess(v));
                    }
                    else
                    {
                        int status = 0;
                        int E_status = 0;

                        for (int i = 0; i < v.Length; i++)
                        {
                            if (v[i] == '-')
                            {
                                if (i == 0 || v[i-1] == 'E')
                                {
                                    continue;
                                }
                                else
                                {
                                    throw new Json.Exception("format error");
                                }
                            }

                            if ((v[i] < '0' || v[i] > '9') && v[i] != '.' && v[i] != 'E')
                            {
                                throw new Json.Exception("format error");
                            }

                            if (v[i] == '.')
                            {
                                status++;
                            }

                            if (v[i] == 'E')
                            {
                                E_status++;
                            }
                        }

                        if (status == 0 && E_status == 0)
                        {
                            _array.Add(Convert.ToInt64(v));
                        }
                        else if (status == 1 && (E_status == 0 || E_status == 1))
                        {
                            _array.Add(Convert.ToDouble(v));
                        }
                        else
                        {
                            throw new Json.Exception("format error");
                        }
                    }

                }

                key = new StringBuilder();
                value = new StringBuilder();

                return _c;
            };

            Func<String, String> parseenum = parsemap;

            Func<object, object> parsepop = (object o) =>{
                _table = null;
                _array = null;
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
                            string _key = key.ToString().Trim();
                            while (_key[0] == '\n' || _key[0] == '\t' || _key[0] == '\0')
                            {
                                _key = _key.Substring(1, _key.Length - 1);
                            }
                            s.Push(_table);
                            _key = _key.Substring(1, _key.Length - 2);
                            _table[_key] = _newtable;
                            _table = null;
                            key = new StringBuilder();
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
                            string _key = key.ToString().Trim();
                            while (_key[0] == '\n' || _key[0] == '\t' || _key[0] == '\0')
                            {
                                _key = _key.Substring(1, _key.Length - 1);
                            }
                            _key = _key.Substring(1, _key.Length - 2);
                            _table[_key] = _newarray;
                            _table = null;
                            key = new StringBuilder();
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
