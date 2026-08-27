using System.Net.WebSockets;
namespace core;

public class WebSocketConnectService
{
    public event Action<INetwork>? OnConnect = null;
    
    private async Task HandleWebSocketAsync(WebSocket webSocket, WebSocketNetwork i)
    {
        var buffer = new byte[1024 * 64];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                try
                {
                    if (!i.OnReceiveData.Receive(buffer.AsSpan(0, result.Count).ToArray()))
                    {
                        Log.Error("WebSocketConnectService OnReceive.OnReceiveData error!");
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("WebSocketConnectService OnReceive.OnReceiveData error:{0}!", e);
                    break;
                }
            }
        }
        webSocket.Abort();
    }
    
    public async Task Connect(string wss)
    {
        try
        {
            var client = new ClientWebSocket();
            var serverUri = new Uri(wss);
            await client.ConnectAsync(serverUri, CancellationToken.None);

            var i = new WebSocketNetwork(client);
            i.T = HandleWebSocketAsync(client, i);
            OnConnect?.Invoke(i);
        }
        catch (WebSocketException ex)
        {
            Log.Error($"Connect failed:{ex}");
        }
    }
}