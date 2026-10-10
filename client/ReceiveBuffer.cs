namespace client;

/// <summary>
/// 拆包：与服务器 <c>core/OnReceive.cs</c> 完全一致的格式 ——
/// 每个包是 [4 字节小端长度][payload]，单包上限 64KB，超过按协议错误处理（断开重连）。
///
/// 服务器那边是"收到多少字节就喂多少"，所以 WebSocket 的每一片（不保证是完整消息）
/// 都可以直接喂进来，由这里负责粘包/半包。
/// </summary>
public class ReceiveBuffer
{
    private const int MaxPacketLen = 65536;

    private byte[] _buffer = new byte[16 * 1024];
    private int _length;

    /// <summary>每拆出一个完整包回调一次</summary>
    public Action<byte[]>? OnPacket;

    /// <summary>返回 false 表示协议错误（长度非法），调用方应当断开</summary>
    public bool Receive(byte[] data, int count)
    {
        if (count <= 0)
        {
            return true;
        }

        EnsureCapacity(_length + count);
        Buffer.BlockCopy(data, 0, _buffer, _length, count);
        _length += count;

        var offset = 0;
        while (true)
        {
            var unread = _length - offset;
            if (unread < 4)
            {
                break;
            }

            var len = _buffer[offset]
                      | _buffer[offset + 1] << 8
                      | _buffer[offset + 2] << 16
                      | _buffer[offset + 3] << 24;
            if (len < 0 || len > MaxPacketLen)
            {
                return false;
            }

            if (unread < len + 4)
            {
                break;
            }

            var packet = new byte[len];
            Buffer.BlockCopy(_buffer, offset + 4, packet, 0, len);
            offset += 4 + len;
            OnPacket?.Invoke(packet);
        }

        if (offset > 0)
        {
            _length -= offset;
            if (_length > 0)
            {
                Buffer.BlockCopy(_buffer, offset, _buffer, 0, _length);
            }
        }

        return true;
    }

    private void EnsureCapacity(int need)
    {
        if (_buffer.Length >= need)
        {
            return;
        }

        var size = _buffer.Length;
        while (size < need)
        {
            size *= 2;
        }

        Array.Resize(ref _buffer, size);
    }
}
