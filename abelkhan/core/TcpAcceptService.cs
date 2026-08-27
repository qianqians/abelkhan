using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;

namespace core;

public class TcpAcceptService(ushort port)
{
    private bool _run = true;
    private Socket? _listenSocket;
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
                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }

                ReadOnlySequence<byte> buffer = result.Buffer;
                if (!i.OnReceiveData.Receive(buffer.ToArray()))
                {
                    Log.Error("TcpAcceptService OnReceive.OnReceiveData error!");
                    break;
                }

                reader.AdvanceTo(buffer.Start, buffer.End);
            }
            catch (Exception e)
            {
                Log.Error("TcpAcceptService OnReceive.OnReceiveData error:{0}!", e);
                break;
            }
        }
        socket.Close();

        await reader.CompleteAsync();
    }

    private async Task RunServerAsync()
    {
        try
        {
            _listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _listenSocket.Listen(128);

            while (_run)
            {
                var socket = await _listenSocket.AcceptAsync();
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
        _t = Task.Factory.StartNew(RunServerAsync, TaskCreationOptions.LongRunning).Unwrap();
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
            _listenSocket?.Close();
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