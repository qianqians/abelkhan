using System.Net.WebSockets;
namespace core;
// ReSharper disable MemberCanBePrivate.Global

public class WebSocketNetwork(WebSocket s) : INetwork
{
    public readonly OnReceive OnReceiveData = new();

    public Task? T;
    private readonly Nito.AsyncEx.AsyncLock _lockObject = new();

    public async Task Send(byte[] data)
    {
        var sendData = new byte[data.Length+4];
        sendData[0] = (byte)(data.Length & 0xff);
        sendData[1] = (byte)(data.Length >> 8 & 0xff);
        sendData[2] = (byte)(data.Length >> 16 & 0xff);
        sendData[3] = (byte)(data.Length >> 24 & 0xff);
        data.CopyTo(sendData, 4);
        
        using (await _lockObject.LockAsync())
        {
            await s.SendAsync(sendData, WebSocketMessageType.Binary, true, CancellationToken.None);
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
                await s.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
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