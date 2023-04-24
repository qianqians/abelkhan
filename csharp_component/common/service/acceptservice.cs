/*
 * acceptservice
 * 2020/6/2
 * qianqians
 */
using System;
using System.Threading.Tasks;
using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;

namespace Abelkhan
{
    public class Acceptservice
    {
        private bool run = true;
        private ushort port;
        private Task _t;

        public Acceptservice(ushort _port)
        {
            port = _port;
        }

        public static event Action<Abelkhan.Ichannel> on_connect;
        public static void onConnect(Channel ch)
        {
            on_connect?.Invoke(ch);
        }

        private async Task ProcessLinesAsync(Socket socket)
        {
            var ch = new Channel(socket);
            Acceptservice.onConnect(ch);

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
                    Log.Log.err("channel_onrecv.on_recv error:{0}!", e);
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