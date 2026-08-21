using core;

namespace hub;

public class GateNetwork(INetwork network)
{
    public async Task Send(byte[] message)
    {
        await network.Send(message);
    }
}