using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace vote
{
    public record FileRepo<T>
    {
        private readonly string _filePath;

        public FileRepo(string filePath, T init)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                Write(init);
            }
        }
        
        public void Write(T obj)
        {
            using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(JsonSerializer.Serialize(obj));
        }

        public T Read()
        {
            using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return JsonSerializer.Deserialize<T>(reader.ReadToEnd()) ?? throw new Exception("Data is null");
        }
    }
}