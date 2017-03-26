using System.Collections;

namespace juggle
{
	public interface Ichannel
    {
        void disconnect();
        ArrayList pop();
        void senddata(byte[] data);

    }
}
