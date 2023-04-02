/*
 * cryptacceptservice
 * qianqians
 * 2020/6/4
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
    public class CryptAcceptConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<CryptAcceptConnectionHandler> _logger;

        public CryptAcceptConnectionHandler(ILogger<CryptAcceptConnectionHandler> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _logger.LogInformation(connection.ConnectionId + " connected");

            var ch = new cryptchannel(connection);
            cryptacceptservice.onConnect(ch);

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

                    cryptacceptservice.onDisconnect(ch);
                    break;
                }
            }

            _logger.LogInformation(connection.ConnectionId + " disconnected");
        }
    }


    public class cryptacceptservice
    {
        private ushort port;
        private IHost _h;
        private Task _t;

        public cryptacceptservice(ushort _port)
        {
            port = _port;
        }

        public static event Action<cryptchannel> on_connect;
        public static void onConnect(cryptchannel ch)
        {
            on_connect?.Invoke(ch);
        }

        public static event Action<cryptchannel> on_disconnect;
        public static void onDisconnect(cryptchannel ch)
        {
            on_disconnect?.Invoke(ch);
        }

        private void RunServerAsync()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost((webHostBuilder) => {
                webHostBuilder
                    .UseKestrel()
                    .ConfigureKestrel((context, options) => {

                        options.ListenLocalhost(port, (builder) => {
                            builder.UseConnectionHandler<CryptAcceptConnectionHandler>();
                        });

                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
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