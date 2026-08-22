using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;

namespace core;

public class TcpAcceptService(ushort port)
{
    private bool _run = true;
    private Task? _t;

    public event Action<INetwork>? OnListenAccept = null;
    private void ListenAccept(INetwork i)
    {
        OnListenAccept?.Invoke(i);
    }

    private async Task ProcessLinesAsync(Socket socket, TcpNetwork i)
    {
        var stream = new NetworkStream(socket);
        var reader = PipeReader.Create(stream);

        while (_run)
        {
            try
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;
                i.OnReceiveData.Receive(buffer.ToArray());
                reader.AdvanceTo(buffer.Start, buffer.End);
            }
            catch (Exception e)
            {
                await i.Close();
                Log.Error("TcpAcceptService OnReceive.OnReceiveData error:{0}!", e);
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
                var i = new TcpNetwork(socket);
                i.T = ProcessLinesAsync(socket, i);
                ListenAccept(i);
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

    public void Close()
    {
        try
        {
            if (_t == null)
            {
                return;
            }
            _run = false;
        }
        catch (Exception ex)
        {
            Log.Error("TcpAcceptService Close error:{0}", ex);
        }
    }

    public async Task Join()
    {
        if (_t == null)
        {
            return;
        }
        await _t;
    }
}