namespace core;

public interface INetwork
{
    Task Send(byte[] data, uint size);
    void OnReceive(Action<byte[]> onReceive);
    Task Close();
}