using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SingleStorage
{
    public class StorageStream
    {
        private BufferedStream _Stream { get; set; }
        private string _Path { get; set; }

        private const int BufferSize = 33554432;

        public StorageStream(string path, bool ReadOnly = true)
        {
            _Path = path;
            if (!File.Exists(path))
            {
                //Empty file
                File.WriteAllBytes(path, new byte[] { 4, 0, 0, 0, 0, 0, 0, 0 });
            }
            Open(ReadOnly);
        }

        public void Close()
        {
            _Stream.Close();
        }

        public void Open(bool ReadOnly = true)
        {
            _Stream?.Close();
            if (ReadOnly)
                _Stream = new BufferedStream(File.OpenRead(_Path), BufferSize);
            else
                _Stream = new BufferedStream(new FileStream(_Path, FileMode.Open, FileAccess.ReadWrite), BufferSize);
        }

        public void Flush() => _Stream.Flush();

        public void Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            _Stream.Seek(offset, origin);
        }

        public uint Pos() => (uint)_Stream.Position;

        public void Cut(long size) => _Stream.SetLength(size);

        public byte[] ReadBytes(int count)
        {
            byte[] value = new byte[count];
            _Stream.Read(value, 0, count);
            return value;
        }

        public short ReadInt16() => BitConverter.ToInt16(ReadBytes(4), 0);
        public ushort ReadUInt16() => BitConverter.ToUInt16(ReadBytes(4), 0);
        public int ReadInt32() => BitConverter.ToInt32(ReadBytes(4), 0);
        public uint ReadUInt32() => BitConverter.ToUInt32(ReadBytes(4), 0);
        public long ReadInt64() => BitConverter.ToInt64(ReadBytes(4), 0);
        public ulong ReadUInt64() => BitConverter.ToUInt64(ReadBytes(4), 0);

        public void Write(byte[] value)
        {
            _Stream.Write(value, 0, value.Length);
        }

        public void Write(dynamic value) => Write(BitConverter.GetBytes(value));
    }
}
