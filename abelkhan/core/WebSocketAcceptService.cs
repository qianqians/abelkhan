using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace core;

public class WebSocketAcceptService
{
    private Task? _t;
    private WebApplication app;
    
    public event Action<INetwork>? OnListenAccept = null;
    private void ListenAccept(INetwork i)
    {
        OnListenAccept?.Invoke(i);
    }

    private async Task HandleWebSocketAsync(WebSocket webSocket, WebSocketNetwork i)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                try
                {
                    if (!i.OnReceiveData.Receive(buffer.AsSpan(0, result.Count).ToArray()))
                    {
                        Log.Error("WebSocketAcceptService OnReceive.OnReceiveData error!");
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("WebSocketAcceptService OnReceive.OnReceiveData error:{0}!", e);
                    break;
                }
            }
        }
        webSocket.Abort();
    }
    
    public WebSocketAcceptService(ushort port, string pfx, string password)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(port, listenOptions =>
            {
                listenOptions.UseHttps(pfx, password);
            });
        });
        
        app = builder.Build();
        app.UseWebSockets();
        
        app.Map("/", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var i = new WebSocketNetwork(webSocket);
                i.T = HandleWebSocketAsync(webSocket, i);
                ListenAccept(i);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        });
    }
    
    public void Start()
    {
        _t = Task.Factory.StartNew(() => { app.Run(); }, TaskCreationOptions.LongRunning);
    }

    public async Task Close()
    {
        try
        {
            if (_t == null)
            {
                return;
            }
            await app.StopAsync();
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