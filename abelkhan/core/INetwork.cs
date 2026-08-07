namespace core;

public interface INetwork
{
    void Send(byte[] data, uint size);
    void OnReceive(Action<byte[]> onReceive);
    void Close();
}