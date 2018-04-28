using System;
using System.IO;
using System.Collections;

namespace common
{
    public class compress_and_encrypt
    {
        static public byte[] CompressAndEncrypt(byte[] input, byte XORKey)
        {
            MemoryStream input_stream = new MemoryStream(input);
            var output_stream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(input_stream);

            byte[] output = new byte[output_stream.Length];
            output_stream.Read(output, 0, output.Length);

            for(int i = 0; i < output.Length; i++)
            {
                output[i] ^= XORKey;
            }

            return output;
        }

        static public byte[] UnEncryptAndUnCompress(byte[] input, byte XORKey)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] ^= XORKey;
            }

            MemoryStream input_stream = new MemoryStream(input);
            var output_stream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(input_stream);

            byte[] output = new byte[output_stream.Length];
            output_stream.Read(output, 0, output.Length);

            return output;
        }
    }
}
