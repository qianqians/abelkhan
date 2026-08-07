using System.Net.Sockets;
namespace core;

public class TcpNetwork : INetwork
{
    public readonly OnReceive OnReceiveData = new();
    private Action<byte[]>? _onReceiveTcpData;

    public Task? T;
    private readonly Socket _socket;
    private readonly Nito.AsyncEx.AsyncLock _lockObject = new();

    public TcpNetwork(Socket s)
    {
        _socket = s;
        OnReceiveData.OnReceiveData += _onReceiveTcpData;
    }
    
    public async Task Send(byte[] data)
    {
        var sendLen = 0;
        using (await _lockObject.LockAsync())
        {
            while (sendLen < data.Length)
            {
                sendLen += _socket.Send(data);
                await Task.Delay(1);
            }
        }
    }
    
    public void OnReceive(Action<byte[]> onReceive)
    {
        _onReceiveTcpData += onReceive;
    }

    public async Task Close()
    {
        try
        {
            using (await _lockObject.LockAsync())
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