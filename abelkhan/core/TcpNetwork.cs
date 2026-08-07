using System.Net.Sockets;

namespace core;

public class TcpNetwork : INetwork
{
    public readonly OnReceive OnReceiveData = new();
    private Action<byte[]>? _onReceiveTcpData;

    public Task? T;
    private readonly Socket _socket;
    private readonly Lock _lockObject = new();

    public TcpNetwork(Socket s)
    {
        _socket = s;
        OnReceiveData.OnReceiveData += _onReceiveTcpData;
    }
    
    public void Send(byte[] data, uint size)
    {
        var sendLen = 0;
        lock (_lockObject)
        {
            while (sendLen < data.Length)
            {
                sendLen += _socket.Send(data, sendLen, data.Length - sendLen, SocketFlags.None);
            }
        }
    }
    
    public void OnReceive(Action<byte[]> onReceive)
    {
        _onReceiveTcpData += onReceive;
    }

    public async void Close()
    {
        try
        {
            lock (_lockObject)
            {
                _socket.Close();
            }

            if (T != null)
            {
                await T;
            }
        }
        catch (Exception ex)
        {
            Log.Error("TcpNetwork Close error:{0}", ex);
        }
    }
}