/*
 * acceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Buffers;

namespace abelkhan
{
    public class AcceptConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<AcceptConnectionHandler> _logger;

        public AcceptConnectionHandler(ILogger<AcceptConnectionHandler> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _logger.LogInformation(connection.ConnectionId + " connected");

            var ch = new channel(connection);
            acceptservice.onConnect(ch);

            while (true)
            {
                try
                {
                    var result = await connection.Transport.Input.ReadAsync();
                    var buffer = result.Buffer;

                    ch._channel_onrecv.on_recv(buffer.ToArray());

                    connection.Transport.Input.AdvanceTo(buffer.End);
                }
                catch (System.Exception e)
                {
                    log.log.err("channel_onrecv.on_recv error:{0}!", e);
                    break;
                }
            }

            _logger.LogInformation(connection.ConnectionId + " disconnected");
        }
    }

    public class TcpStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }

    public class acceptservice
    {
        private ushort port;
        private IHost _h;
        private Task _t;

        public acceptservice(ushort _port)
        {
            port = _port;
        }

        public static event Action<abelkhan.Ichannel> on_connect;
        public static void onConnect(channel ch)
        {
            on_connect?.Invoke(ch);
        }

        private void RunServerAsync()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost((webHostBuilder) => {
                webHostBuilder
                    .UseKestrel()
                    .ConfigureKestrel((context, options) =>
                    {

                        options.ListenLocalhost(port, (builder) =>
                        {
                            builder.UseConnectionHandler<AcceptConnectionHandler>();
                        });

                    })
                    .UseStartup<TcpStartup>();
            });

            _h = hostBuilder.Build();
            _h.Run();
        }

        public void start()
        {
            _t = new Task(RunServerAsync, TaskCreationOptions.LongRunning);
            _t.Start();
        }

        public async void close()
        {
            await _h.StopAsync();
        }
    }
}