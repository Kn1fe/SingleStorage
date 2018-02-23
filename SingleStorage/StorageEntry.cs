using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SingleStorage
{
    public class StorageEntry
    {
        public uint Pos { get; set; }
        public int Size { get; set; }
        public int CSize { get; set; }
        public string Path { get; set; }

        public StorageEntry()
        {
            Pos = 0;
            Size = 0;
            CSize = 0;
            Path = "";
        }

        public StorageEntry(byte[] data)
        {
            Read(data);
        }

        public void Read(byte[] data)
        {
            Pos = BitConverter.ToUInt32(data, 0);
            Size = BitConverter.ToInt32(data, 4);
            CSize = BitConverter.ToInt32(data, 8);
            int len = BitConverter.ToInt32(data, 12);
            Path = Encoding.Unicode.GetString(data, 16, len);
        }

        public byte[] Write()
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(Pos), 0, 4);
            ms.Write(BitConverter.GetBytes(Size), 0, 4);
            ms.Write(BitConverter.GetBytes(CSize), 0, 4);
            ms.Write(BitConverter.GetBytes(Encoding.Unicode.GetByteCount(Path)), 0, 4);
            ms.Write(Encoding.Unicode.GetBytes(Path), 0, Encoding.Unicode.GetByteCount(Path));
            return ms.ToArray();
        }
    }
}
