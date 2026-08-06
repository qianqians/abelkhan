using System.Buffers;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipelines;
using System.Runtime.InteropServices.ComTypes;

namespace core;

public class TcpConnectService
{
    public static event Action<INetwork>? OnConnect = null;
    
    private async Task ProcessLinesAsync(Socket socket, TcpNetwork i)
    {
        var stream = new NetworkStream(socket);
        var reader = PipeReader.Create(stream);

        while (true)
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
    
    public void Connect(IPAddress address, short port)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        s.Connect(address, port);
        var i = new TcpNetwork(s);
        i.T = ProcessLinesAsync(s, i);
        OnConnect?.Invoke(i);
    }
}