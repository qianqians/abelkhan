using System.Collections;

namespace juggle
{
	public interface Ichannel
    {
		ArrayList pop();
        void senddata(byte[] data);

    }
}
