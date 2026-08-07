using System.Net.WebSockets;
namespace core;
// ReSharper disable MemberCanBePrivate.Global

public class WebSocketNetwork : INetwork
{
    public readonly OnReceive OnReceiveData = new();
    private Action<byte[]>? _onReceiveWebSocketData;

    public Task? T;
    private readonly WebSocket _socket;
    private readonly Nito.AsyncEx.AsyncLock _lockObject = new();

    public WebSocketNetwork(WebSocket s)
    {
        _socket = s;
        OnReceiveData.OnReceiveData += _onReceiveWebSocketData;
    }
    
    public async Task Send(byte[] data)
    {
        using (await _lockObject.LockAsync())
        {
            await _socket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
    
    public void OnReceive(Action<byte[]> onReceive)
    {
        _onReceiveWebSocketData += onReceive;
    }

    public async Task Close()
    {
        try
        {
            using (await _lockObject.LockAsync())
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
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