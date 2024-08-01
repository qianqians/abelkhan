/*
 * cryptacceptservice
 * qianqians
 * 2020/6/4
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.Threading;
using System.Text;

namespace Service
{
    public class AbelkhanHttpRequest
    {
        public HttpRequest req;
        public HttpResponse rsp;
        public byte[] Content;
        public int length;

        public AbelkhanHttpRequest(HttpRequest _req, HttpResponse _rsp, byte[] _Content, int _length) {
            req = _req;
            rsp = _rsp;
            Content = _Content;
            length = _length;
        }

        public ValueTask Response(int status /*Microsoft.AspNetCore.Http.StatusCodes*/, Dictionary<string, string> headers, byte[] buf) {
            try {
                foreach (var h in headers) {
                    rsp.Headers[h.Key] = h.Value;
                }

                rsp.StatusCode = status;

                return rsp.Body.WriteAsync(buf);

            } catch (Exception ex) {
                Log.Log.err("Response Exception:{0}", ex);
            } finally {
            }
            return ValueTask.CompletedTask;
        }
    }

    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
        }

        private static int lcount = 0;
        private static long _recvStatTick = Timerservice.Tick + 1000;
        private static long _lastStatTick = Timerservice.Tick + 1000;
        public void Configure(IApplicationBuilder app) {
            app.Run(async (context) =>
            {
                var begin = Timerservice.Tick;

                int count = Interlocked.Add(ref lcount, 1);
                if (Timerservice.Tick >= _recvStatTick) {
                    Interlocked.And(ref lcount, -count);
                    Log.Log.info("Connect statistics: {0} messages in {1} ms", count, Timerservice.Tick - _lastStatTick);
                    _lastStatTick = Timerservice.Tick;
                    _recvStatTick = Timerservice.Tick + 1000;
                }

                Func<AbelkhanHttpRequest, Task> cb = null;
                if (context.Request.Method == HttpMethods.Get)
                {
                    HttpService.TryGetGetCallback(context.Request.Path, out cb);
                }
                else if (context.Request.Method == HttpMethods.Post)
                {
                    HttpService.TryGetPostCallback(context.Request.Path, out cb);
                }
                else if (context.Request.Method == HttpMethods.Options)
                {
                    foreach (var h in HttpService.buildCrossHeaders())
                    {
                        context.Response.Headers[h.Key] = h.Value;
                    }
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.Body.Write(Encoding.UTF8.GetBytes(""));
                    return;
                }
                if (cb == null) {
                    return;
                }

                byte[] buf = null;
                int length = 0;
                try {
                    if (context.Request.ContentLength != null) {
                        length = (int)context.Request.ContentLength;
                        buf = ArrayPool<byte>.Shared.Rent(length);
                        int offset = 0;
                        while (true) {
                            var len = await context.Request.Body.ReadAsync(buf, offset, length - offset);
                            if (len == 0) {
                                break;
                            }
                            offset += len;
                        }
                    }
                    await cb(new AbelkhanHttpRequest(context.Request, context.Response, buf, length));
                } catch (Exception ex) {
                    Log.Log.err("process http req ex:{0}", ex);
                } finally {
                    if (buf != null) {
                        ArrayPool<byte>.Shared.Return(buf);
                    }

                    var tick = Timerservice.Tick - begin;
                    if (tick > 1000) {
                        Log.Log.err("Timeout: elapsed_ticks={0}", tick);
                    }
                }
            });
        }
    }

    public class HttpService
    {
        public static Dictionary<string, Func<AbelkhanHttpRequest, Task> > get_callbacks = new Dictionary<string, Func<AbelkhanHttpRequest, Task> >();
        public static Dictionary<string, Func<AbelkhanHttpRequest, Task> > post_callbacks = new Dictionary<string, Func<AbelkhanHttpRequest, Task> >();
 

        private string _host;
        private int _port;
        private IHost _h;
        private Task _t;

        public HttpService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        private static Dictionary<string, string>  headers = new Dictionary<string, string> { 
            { "Content-Type", "application/json; charset=utf-8" },
            { "Access-Control-Allow-Origin", "*" }, {"Access-Control-Allow-Headers", "XL-Token, Content-Type" },
            { "Access-Control-Allow-Methods", "POST, GET, OPTIONS"} };
        public static Dictionary<string, string> buildCrossHeaders()
        {
            return headers;
        }

        public static void get(string uri, Func<AbelkhanHttpRequest, Task> callback)
        {
            get_callbacks.Add(uri, callback);
        }

        public static void post(string uri, Func<AbelkhanHttpRequest, Task> callback)
        {
            post_callbacks.Add(uri, callback);
        }

        public static bool TryGetPostCallback(string uri, out Func<AbelkhanHttpRequest, Task> cb)
        {
            return post_callbacks.TryGetValue(uri, out cb);
        }

        public static bool TryGetGetCallback(string uri, out Func<AbelkhanHttpRequest, Task> cb)
        {
            return get_callbacks.TryGetValue(uri, out cb);
        }

        private void RunServerAsync()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost((webHostBuilder) => {
                webHostBuilder
                    .UseKestrel()
                    .ConfigureKestrel((context, options) => {

                        options.ListenAnyIP(_port, (listenOptions) => {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });

                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();
            });

            _h = hostBuilder.Build();
            _h.Run();
        }

        public void run() {
            _t = Task.Factory.StartNew(RunServerAsync, TaskCreationOptions.LongRunning);
            _t.Start();
        }

        public async void close() {
            await _h.StopAsync();
        }
    }
}