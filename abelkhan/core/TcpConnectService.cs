using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;

namespace core;

public class TcpConnectService
{
    public event Action<string, INetwork>? OnConnect = null;
    private bool _run = true;
    
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
                    Log.Error("TcpConnectService OnReceive.OnReceiveData error!");
                    break;
                }

                reader.AdvanceTo(buffer.Start, buffer.End);
            }
            catch (Exception e)
            {
                Log.Error("TcpConnectService OnReceive.OnReceiveData error:{0}!", e);
                break;
            }
        }
        socket.Close();

        await reader.CompleteAsync();
    }
    
    public void Connect(string id, IPAddress address, ushort port)
    {
        try
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(address, port);
            var i = new TcpNetwork(s);
            i.T = ProcessLinesAsync(s, i);
            OnConnect?.Invoke(id, i);
        }
        catch (Exception e)
        {
            Log.Error("TcpConnectService OnConnect error:{0}!", e);
        }
    }
    
    public void Close()
    {
        try
        {
            _run = false;
        }
        catch (Exception ex)
        {
            Log.Error("TcpConnectService Close error:{0}", ex);
        }
    }
}