/*
 * cryptacceptservice
 * qianqians
 * 2020/6/4
 */
using System;
using System.Threading.Tasks;
using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;

namespace abelkhan
{
    public class cryptacceptservice
    {
        private bool run = true;
        private ushort port;
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

        private async Task ProcessLinesAsync(Socket socket)
        {
            var ch = new channel(socket);
            acceptservice.onConnect(ch);

            var stream = new NetworkStream(socket);
            var reader = PipeReader.Create(stream);

            while (run)
            {

                try
                {
                    ReadResult result = await reader.ReadAsync();
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    ch._channel_onrecv.on_recv(buffer.ToArray());

                    reader.AdvanceTo(buffer.Start, buffer.End);
                }
                catch (System.Exception e)
                {
                    log.log.err("channel_onrecv.on_recv error:{0}!", e);
                    break;
                }
            }

            await reader.CompleteAsync();
        }

        private async void RunServerAsync()
        {
            var listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Loopback, port));
            listenSocket.Listen(128);

            while (run)
            {
                var socket = await listenSocket.AcceptAsync();
                _ = ProcessLinesAsync(socket);
            }
        }

        public void start()
        {
            _t = new Task(RunServerAsync, TaskCreationOptions.LongRunning);
            _t.Start();
        }

        public async void close()
        {
            run = false;
            await _t;
        }
    }
}