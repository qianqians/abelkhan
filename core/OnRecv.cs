
namespace core;

public class OnReceive
{
    private static readonly Microsoft.IO.RecyclableMemoryStreamManager StreamPool = new();
    private readonly Microsoft.IO.RecyclableMemoryStream _receiveBuf = StreamPool.GetStream();

    public readonly Action<byte[]>? OnReceiveData = null;
    public void Receive(byte[] data)
    {
        _receiveBuf.Write(data, 0, data.Length);
        var bufferLen = _receiveBuf.Length;

        var offset = 0;
        var underBuf = _receiveBuf.GetBuffer();
        while (true)
        {
            var unread = bufferLen - offset;
            if (unread < 4)
            {
                break;
            }

            int len = underBuf[offset];
            len |= underBuf[offset + 1] << 8;
            len |= underBuf[offset + 2] << 16;
            len |= underBuf[offset + 3] << 24;

            if (unread < len + 4)
            {
                break;
            }

            offset += 4;
            using var tmp = StreamPool.GetStream();
            tmp.Write(underBuf, offset, len);
            tmp.Position = 0;
            OnReceiveData?.Invoke(tmp.GetBuffer());
            offset += len;
        }

        if (offset > 4)
        {
            var pos = bufferLen - offset;
            Buffer.BlockCopy(underBuf, offset, underBuf, 0, (int)pos);
            _receiveBuf.Seek(pos, SeekOrigin.Begin);
            _receiveBuf.SetLength(pos);
        }
    }
}