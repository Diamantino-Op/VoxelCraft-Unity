using ZstdNet;

namespace VoxelCraft.Compression
{
    public static class CompressionManager
    {
        public static byte[] Compress(byte[] data, byte[] dict)
        {
            using var options = new CompressionOptions(dict, compressionLevel: 5);
            using var compressor = new Compressor(options);
            return compressor.Wrap(data);
        }

        public static byte[] Decompress(byte[] data, byte[] dict)
        {
            using var options = new DecompressionOptions(dict);
            using var decompressor = new Decompressor(options);
            return decompressor.Unwrap(data);
        }
    }
}