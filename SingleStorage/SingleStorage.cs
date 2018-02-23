using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SingleStorage
{
    public class SingleStorage
    {
        public List<StorageEntry> Files { get; set; }
        public bool CompressFiles { get; set; }
        public bool CompressEntrys { get; set; }

        private StorageStream _Stream { get; set; }

        public SingleStorage()
        {
            Files = new List<StorageEntry>();
            CompressFiles = false;
            CompressEntrys = true;
        }

        public SingleStorage(string path)
        {
            Files = new List<StorageEntry>();
            CompressFiles = false;
            CompressEntrys = true;
            Load(path);
        }

        public void Load(string path)
        {
            _Stream = new StorageStream(path);
            uint FileTableAdress = _Stream.ReadUInt32();
            _Stream.Seek(FileTableAdress, SeekOrigin.Begin);
            uint count = _Stream.ReadUInt32();
            for (uint i = 0; i < count; ++i)
            {
                int size = _Stream.ReadInt32();
                int csize = _Stream.ReadInt32();
                Files.Add(new StorageEntry(size != csize ? Compressor.Decompress(_Stream.ReadBytes(csize), size) : _Stream.ReadBytes(csize)));
            }
        }

        public bool Contains(string path) => Files.Where(x => x.Path == path).Count() > 0 ? true : false;
        public StorageEntry GetFileEntry(string path) => Files.Where(x => x.Path == path).First();
        
        public byte[] GetFile(string path)
        {
            if (!Contains(path))
                return new byte[0];
            var entry = GetFileEntry(path);
            _Stream.Seek(entry.Pos);
            return entry.Size != entry.CSize ? Compressor.Decompress(_Stream.ReadBytes(entry.CSize), entry.Size) : _Stream.ReadBytes(entry.CSize);
        }

        public void InsertMulti(IDictionary<string, byte[]> files)
        {
            _Stream.Open(false);
            _Stream.Seek(0);
            uint FileTableAdress = _Stream.ReadUInt32();
            _Stream.Cut(FileTableAdress);
            foreach (var file in files)
            {
                Insert(file.Value, file.Key, false);
            }
            DumpFileTable();
        }

        public void Insert(byte[] file, string path, bool DumpFileTable = true)
        {
            int len = file.Length;
            byte[] data = CompressFiles ? Compressor.Compress(file) : file;
            int clen = data.Length;
            if (Contains(path))
            {
                StorageEntry entry = GetFileEntry(path);
                if (clen <= entry.CSize)
                    _Stream.Seek(entry.Pos);
                else
                    _Stream.Seek(0, SeekOrigin.End);
                Files.Add(new StorageEntry()
                {
                    Pos = _Stream.Pos(),
                    Path = path,
                    Size = len,
                    CSize = clen
                });
                _Stream.Write(data);
            }
            else
            {
                _Stream.Seek(0, SeekOrigin.End);
                Files.Add(new StorageEntry()
                {
                    Pos = _Stream.Pos(),
                    Path = path,
                    Size = len,
                    CSize = clen
                });
                _Stream.Write(data);
            }
            if (DumpFileTable)
                this.DumpFileTable();
        }

        public void DumpFileTable()
        {
            _Stream.Seek(0, SeekOrigin.End);
            uint FileTalbe = _Stream.Pos();
            _Stream.Write(Files.Count);
            foreach (var file in Files)
            {
                byte[] entry = file.Write();
                _Stream.Write(entry.Length);
                entry = CompressEntrys ? Compressor.Compress(entry) : entry;
                _Stream.Write(entry.Length);
                _Stream.Write(entry);
            }
            _Stream.Seek(0);
            _Stream.Write(FileTalbe);
            _Stream.Flush();
        }
    }
}
