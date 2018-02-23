using ComponentAce.Compression.Libs.zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SingleStorage
{
    class Compressor
    {
        public static int CompressionLevel = 9;

        public static byte[] Compress(byte[] data, int level = -1)
        {
            if (level == -1)
                level = CompressionLevel;
            MemoryStream ms = new MemoryStream();
            ZOutputStream zos = new ZOutputStream(ms, CompressionLevel);
            CopyStream(new MemoryStream(data), zos, data.Length);
            zos.finish();
            return ms.ToArray().Length < data.Length ? ms.ToArray() : data;
        }

        public static byte[] Decompress(byte[] data, int size)
        {
            byte[] output = new byte[size];
            ZOutputStream zos = new ZOutputStream(new MemoryStream(output));
            try
            {
                CopyStream(new MemoryStream(data), zos, size);
            }
            catch { throw new Exception("Invalid zlib data"); }
            return output;
        }

        private static void CopyStream(Stream input, Stream output, int Size)
        {
            try
            {
                byte[] buffer = new byte[Size];
                int len;
                while ((len = input.Read(buffer, 0, Size)) > 0)
                {
                    output.Write(buffer, 0, len);
                }
                output.Flush();
            }
            catch
            {
                Console.WriteLine("\nBad zlib data");
            }
        }
    }
}
