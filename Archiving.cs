using System.IO;
using System.IO.Compression;

namespace LabDeCSharp
{
    class Archiving
    {
        public static void Acrhive(string file)
        {
            using (FileStream sourceStream = new FileInfo(file).OpenRead())
            {
                using (FileStream targetStream = File.Create(file + ".gz"))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }

        public static void Unarchive(string file)
        {
            using (FileStream sourceStream = new FileStream(file, FileMode.Open))
            {
                using (FileStream targetStream = File.Create(Path.GetDirectoryName(file) + '\\' + Path.GetFileNameWithoutExtension(file)))
                {
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
            }
        }
    }
}