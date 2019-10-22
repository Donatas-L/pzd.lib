using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;

namespace pzd.lib.utils {
  [PublicAPI] public static class GZip {
    public static byte[] compress(byte[] data) {
      using (var compressedStream = new MemoryStream())
      using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress)) {
        zipStream.Write(data, 0, data.Length);
        zipStream.Close();
        return compressedStream.ToArray();
      }
    }

    public static byte[] decompress(byte[] data) {
      using (var compressedStream = new MemoryStream(data))
      using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
      using (var resultStream = new MemoryStream()) {
        zipStream.CopyTo(resultStream);
        return resultStream.ToArray();
      }
    }
  }
}