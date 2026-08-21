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
        using (await _lockObject.LockAsync())
        {
            await s.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
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