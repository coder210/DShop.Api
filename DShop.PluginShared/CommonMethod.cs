using System.IO.Compression;

namespace DShop.PluginShared
{
    public class CommonMethod
    {
        public static string GetImageRelativePath(string filename)
        {
            return "/images/" + filename;
        }

        public static string GetImageDirectory(string basePath)
        {
            return basePath + "/images/";
        }

        public static byte[] CompressBytes(byte[] input)
        {
            using var outputStream = new MemoryStream();
            using (var gzip = new GZipStream(outputStream, CompressionLevel.Optimal))
            {
                gzip.Write(input, 0, input.Length);
            }
            return outputStream.ToArray();
        }

    }
}
