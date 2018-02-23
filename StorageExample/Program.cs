using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StorageExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"D:\PWI_en\element\info\");
            SingleStorage.SingleStorage ss = new SingleStorage.SingleStorage("Cache");
            Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
            foreach (string file in files)
            {
                dictionary.Add(Path.GetFileName(file), File.ReadAllBytes(file));
            }
            ss.InsertMulti(dictionary);
            foreach (var file in ss.Files)
            {
                byte[] data = ss.GetFile(file.Path);
                File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tmp", file.Path), data);
            }
        }
    }
}
