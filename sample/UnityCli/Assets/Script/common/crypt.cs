/*
 * channel
 * 2020/6/2
 * qianqians
 */

namespace abelkhan
{
    public class crypt
    {
        public static void crypt_func(byte[] data, int offset, int limit)
        {
            var len = data.Length;
			
			byte xor_key0  = (byte)(len & 0xff);
			byte xor_key1 = (byte)((len >> 8) & 0xff);
			byte xor_key2 = (byte)((len >> 16) & 0xff);
			byte xor_key3 = (byte)((len >> 24) & 0xff);
			
			var base_xor = 0;
			if (xor_key0 != 0)
			{
				base_xor = xor_key0;
			}
			else if (xor_key1 != 0) 
			{
				base_xor = xor_key1;
			}
			else if (xor_key2 != 0) 
			{
				base_xor = xor_key2;
			}
			else if (xor_key3 != 0) 
			{
				base_xor = xor_key3;
			}
			
			if (xor_key0 == 0)
            {
                xor_key0 = (byte)(base_xor + base_xor % 3);
            }
            if (xor_key1 == 0)
            {
                xor_key1 = (byte)(base_xor + base_xor % 7);
            }
            if (xor_key2 == 0)
            {
                xor_key2 = (byte)(base_xor + base_xor % 13);
            }
            if (xor_key3 == 0)
            {
                xor_key3 = (byte)(base_xor + base_xor % 17);
            }

            for (var i = offset; i < limit; ++i)
            {
                var k = i % 4;
                if (k == 0)
                {
                    data[i] ^= xor_key0;
                }
                else if (k == 1)
                {
                    data[i] ^= xor_key1;
                }
                else if (k == 2)
                {
                    data[i] ^= xor_key2;
                }
                else if (k == 3)
                {
                    data[i] ^= xor_key3;
                }
            }
        }

        public static void crypt_func_send(byte[] data)
        {
            byte xor_key0 = data[0];
            byte xor_key1 = data[1];
            byte xor_key2 = data[2];
            byte xor_key3 = data[3];
			
			var base_xor = 0;
			if (xor_key0 != 0)
			{
				base_xor = xor_key0;
			}
			else if (xor_key1 != 0) 
			{
				base_xor = xor_key1;
			}
			else if (xor_key2 != 0) 
			{
				base_xor = xor_key2;
			}
			else if (xor_key3 != 0) 
			{
				base_xor = xor_key3;
			}
			
            if (xor_key0 == 0)
            {
                xor_key0 = (byte)(base_xor + base_xor % 3);
            }
            if (xor_key1 == 0)
            {
                xor_key1 = (byte)(base_xor + base_xor % 7);
            }
            if (xor_key2 == 0)
            {
                xor_key2 = (byte)(base_xor + base_xor % 13);
            }
            if (xor_key3 == 0)
            {
                xor_key3 = (byte)(base_xor + base_xor % 17);
            }

            for (var i = 4; i < data.Length; ++i)
            {
                var k = i % 4;
                if (k == 0)
                {
                    data[i] ^= xor_key0;
                }
                else if (k == 1)
                {
                    data[i] ^= xor_key1;
                }
                else if (k == 2)
                {
                    data[i] ^= xor_key2;
                }
                else if (k == 3)
                {
                    data[i] ^= xor_key3;
                }
            }
        }
    }
}