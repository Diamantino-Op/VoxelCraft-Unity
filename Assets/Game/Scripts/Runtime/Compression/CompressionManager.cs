using ZstdSharp;

namespace VoxelCraft.Compression
{
    public static class CompressionManager
    {
        public static byte[] Compress(byte[] data)
        {
            using var compressor = new Compressor(5);
            return compressor.Wrap(data).ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            using var decompressor = new Decompressor();
            return decompressor.Unwrap(data).ToArray();
        }
    }
}