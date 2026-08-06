using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;

namespace core;

public class TcpAcceptService(ushort port)
{
    private bool _run = true;
    private Task? _t;

    public static event Action<INetwork>? OnListenAccept = null;
    private static void ListenAccept(INetwork i)
    {
        OnListenAccept?.Invoke(i);
    }

    private async ValueTask ProcessLinesAsync(Socket socket)
    {
        var i = new TcpNetwork(socket);
        ListenAccept(i);

        var stream = new NetworkStream(socket);
        var reader = PipeReader.Create(stream);

        while (_run)
        {

            try
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                i.OnReceiveData.OnReceiveData?.Invoke(buffer.ToArray());

                reader.AdvanceTo(buffer.Start, buffer.End);
            }
            catch (System.Exception e)
            {
                Log.Error("OnReceive.OnReceiveData error:{0}!", e);
                break;
            }
        }

        await reader.CompleteAsync();
    }

    private async void RunServerAsync()
    {
        try
        {
            var listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            listenSocket.Listen(128);

            while (_run)
            {
                var socket = await listenSocket.AcceptAsync();
                _ = ProcessLinesAsync(socket);
            }
        }
        catch (Exception ex)
        {
            Log.Error("RunServerAsync error:{0}", ex);
        }
    }

    public void Start()
    {
        _t = Task.Factory.StartNew(RunServerAsync, TaskCreationOptions.LongRunning);
    }

    public async void Close()
    {
        try
        {
            if (_t == null)
            {
                return;
            }
            
            _run = false;
            await _t;
        }
        catch (Exception ex)
        {
            Log.Error("Close error:{0}", ex);
        }
    }
}