using System;
using System.IO;

namespace vote.Participant
{
    public static class CommonPaths
    {
        public static readonly string BasePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/vote";

        static CommonPaths()
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }
    }
}