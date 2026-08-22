using System.Net.Sockets;
namespace core;

public class TcpNetwork(Socket s) : INetwork
{
    public readonly OnReceive OnReceiveData = new();

    public Task? T;
    private readonly Nito.AsyncEx.AsyncLock _lockObject = new();

    public async Task Send(byte[] data)
    {
        var sendLen = 0;
        using (await _lockObject.LockAsync())
        {
            sendLen += s.Send(data, sendLen, data.Length, SocketFlags.None);
            while (sendLen < data.Length)
            {
                await Task.Delay(1);
                sendLen += s.Send(data, sendLen, data.Length - sendLen, SocketFlags.None);
            }
        }
    }
    
    public void OnReceive(Action<byte[]> onReceive)
    {
        OnReceiveData.OnReceiveData += onReceive;
    }

    public async Task Close()
    {
        try
        {
            using (await _lockObject.LockAsync())
            {
                s.Close();
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