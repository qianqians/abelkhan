using Abelkhan;
using System;
using System.Collections;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dy
{
    public class code2Session
    {
        public long error;
        public string session_key;
        public string openid;
        public string anonymous_openid;
    }

    public class code2SessionEx : System.Exception
    {
        public long error;
        public long errcode;
        public string errmsg;
        public string message;
    }

    public class dySdk
    {
        public static async ValueTask<code2Session> code2Session(string appid, string secret, string code, string anonymous_code)
        {
            try
            {
                string url;
                if (string.IsNullOrEmpty(anonymous_code))
                {
                    url = $"https://minigame.zijieapi.com/mgplatform/api/apps/jscode2session?appid={appid}&secret={secret}&code={code}";
                }
                else
                {
                    url = $"https://minigame.zijieapi.com/mgplatform/api/apps/jscode2session?appid={appid}&secret={secret}&code={code}&anonymous_code={anonymous_code}";
                } 
                Log.Log.trace("on_player_login:{0}", url);
                var result = await HttpClientWrapper.GetRspAsync(url);

                if (result != null && result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var ret = await result.Content.ReadAsStringAsync();
                    Log.Log.trace("jscode2session:{0}", ret);
                    var json_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(ret);
                    var err = (long)json_obj["error"];
                    if (err == 0)
                    {
                        var ret_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<code2Session>(ret);
                        ret_obj.openid = $"dy_{ret_obj.openid}";
                        ret_obj.anonymous_openid = $"dy_{ret_obj.anonymous_openid}";
                        return ret_obj;
                    }
                    else
                    {
                        var ex = Newtonsoft.Json.JsonConvert.DeserializeObject<code2SessionEx>(ret);
                        throw ex;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err($"{ex}");
                throw;
            }

            return null;
        }
    }
}
