using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.Json;

namespace vote
{
    public record ListFileRepo<T>
    {
        private readonly string _filePath;

        public ListFileRepo(string filePath)
        {
            _filePath = filePath;
            Read(); // To Create File
        }
        
        public void Append(T obj)
        {
            using var stream = new FileStream(_filePath, FileMode.Append, FileAccess.Write);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(JsonSerializer.Serialize(obj)+Environment.NewLine);
        }

        public ImmutableList<T> Read()
        {
            using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var elements = new List<T>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine() ?? throw new Exception("Data is null");
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var deserialize = JsonSerializer.Deserialize<T>(line) ?? throw new Exception("Problem deserializing");
                elements.Add(deserialize);
            }
            return elements.ToImmutableList();
        }
        
    }
}