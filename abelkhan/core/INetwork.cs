namespace core;

public interface INetwork
{
    Task Send(byte[] data);
    void OnReceive(Action<byte[]> onReceive);
    Task Close();
}