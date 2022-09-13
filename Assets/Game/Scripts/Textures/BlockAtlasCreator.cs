using VoxelCraft.World.Block;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace VoxelCraft.Textures
{
    public class BlockAtlasCreator
    {
        public static Atlas generateAtlas(Dictionary<int, Block> blocks)
        {
            int blockSize = 16;
            int atlasSizeInBlocks = (int)Mathf.Sqrt(blocks.Count);

            if (!(atlasSizeInBlocks % 2 == 0))
                atlasSizeInBlocks += 1;

            int atlasSize = blockSize * atlasSizeInBlocks;

            float tUnit = 1f / atlasSizeInBlocks;

            int indexX = 0;
            int indexY = atlasSizeInBlocks - 1;

            List<Texture2D> sortedTextures = new List<Texture2D>();
            Dictionary<string, Vector2> uvs = new Dictionary<string, Vector2>();

            foreach(KeyValuePair<int, Block> block in blocks)
            {
                foreach(Texture2D texture in block.Value.textures)
                {
                    if (!uvs.ContainsKey(texture.name))
                    {
                        texture.alphaIsTransparency = true;

                        if (texture.width == blockSize && texture.height == blockSize)
                            sortedTextures.Add(texture);
                        else
                        {
                            texture.Reinitialize(blockSize, blockSize);
                            sortedTextures.Add(texture);
                        }

                        uvs.Add(texture.name, new Vector2(indexX, indexY));

                        indexX++;

                        if (indexX > atlasSizeInBlocks - 1)
                        {
                            indexX = 0;
                            indexY--;
                        }
                    }
                }
            }

            Texture2D atlas = new Texture2D(atlasSize, atlasSize);
            Color[] pixels = new Color[atlasSize * atlasSize];

            for(int x = 0; x < atlasSize; x++)
            {
                for (int y = 0; y < atlasSize; y++)
                {
                    int currentBlockX = x / blockSize;
                    int currentBlockY = y / blockSize;

                    int index = currentBlockY * atlasSizeInBlocks + currentBlockX;

                    if (index < sortedTextures.Count)
                        pixels[(atlasSize - y - 1) * atlasSize + x] = sortedTextures[index].GetPixel(x, blockSize - y - 1);
                    else
                        pixels[(atlasSize - y - 1) * atlasSize + x] = new Color(255f, 255f, 255f, 0f);
                }
            }

            atlas.name = "Block Atlas";

            atlas.SetPixels(pixels);

            atlas.alphaIsTransparency = true;

            atlas.filterMode = FilterMode.Point;

            atlas.Apply();

            return new Atlas(atlas, uvs, tUnit);
        }
    }

    public class Atlas
    {
        public Texture2D atlas;
        public Dictionary<string, Vector2> uvDictionary;
        public float tUnit;

        public Atlas(Texture2D atlas, Dictionary<string, Vector2> uvDictionary, float tUnit)
        {
            this.atlas = atlas;
            this.uvDictionary = uvDictionary;
            this.tUnit = tUnit;
        }
    }
}
