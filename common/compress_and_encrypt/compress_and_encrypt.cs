using System;
using System.IO;
using System.Collections;

namespace common
{
    public class compress_and_encrypt
    {
        static public byte[] CompressAndEncrypt(byte[] input, byte XORKey)
        {
            MemoryStream input_stream = new MemoryStream();

            var output_stream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(input_stream);
            output_stream.Write(input, 0, input.Length);
            output_stream.Close();

            byte[] output = input_stream.ToArray();

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
            StreamReader rader = new StreamReader(output_stream, System.Text.Encoding.UTF8);
            var tmp = rader.ReadToEnd();
            byte[] output = System.Text.Encoding.UTF8.GetBytes(tmp);

            return output;
        }
    }
}
