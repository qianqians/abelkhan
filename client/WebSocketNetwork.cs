using System.Net.WebSockets;

namespace client;

/// <summary>
/// 客户端 WebSocket 传输：发送时加 4 字节小端长度前缀，接收时交给 <see cref="ReceiveBuffer"/> 拆包。
/// 读循环结束（对端关闭 / 网络异常 / 本地 Close）会触发 <see cref="OnClose"/>，用来做断线重连。
/// </summary>
public class WebSocketNetwork(ClientWebSocket socket, ReceiveBuffer receiveBuffer)
{
    /// <summary>连接断开（只会触发一次）</summary>
    public event Action? OnClose;

    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private Task? _readTask;
    private volatile bool _closed;
    private int _closeNotified;

    public bool IsOpen => !_closed && socket.State == WebSocketState.Open;

    public void Start()
    {
        _readTask = Task.Factory.StartNew(ReadLoop, TaskCreationOptions.LongRunning).Unwrap();
    }

    private async Task ReadLoop()
    {
        var buffer = new byte[16 * 1024];
        try
        {
            while (!_closed && socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                if (result.MessageType != WebSocketMessageType.Binary || result.Count <= 0)
                {
                    continue;
                }

                if (!receiveBuffer.Receive(buffer, result.Count))
                {
                    Log.Error("client websocket receive: bad packet length!");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            if (!_closed)
            {
                Log.Error($"client websocket recv ex:{e.Message}");
            }
        }
        finally
        {
            try
            {
                socket.Abort();
            }
            catch
            {
                // ignore
            }

            _closed = true;
            NotifyClose();
        }
    }

    public async Task Send(byte[] data)
    {
        var sendData = new byte[data.Length + 4];
        sendData[0] = (byte)(data.Length & 0xff);
        sendData[1] = (byte)(data.Length >> 8 & 0xff);
        sendData[2] = (byte)(data.Length >> 16 & 0xff);
        sendData[3] = (byte)(data.Length >> 24 & 0xff);
        data.CopyTo(sendData, 4);

        await _sendLock.WaitAsync();
        try
        {
            if (_closed || socket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException($"client websocket not open:{socket.State}");
            }

            await socket.SendAsync(new ArraySegment<byte>(sendData), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    /// <summary>本地关闭；由读循环结束触发的关闭不会再回调 OnClose（幂等）</summary>
    public Task Close()
    {
        if (_closed)
        {
            return Task.CompletedTask;
        }

        _closed = true;
        try
        {
            socket.Abort();
        }
        catch
        {
            // ignore
        }

        NotifyClose();
        return Task.CompletedTask;
    }

    private void NotifyClose()
    {
        if (Interlocked.Exchange(ref _closeNotified, 1) == 0)
        {
            OnClose?.Invoke();
        }
    }
}
