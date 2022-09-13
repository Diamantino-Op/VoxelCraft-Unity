using System;
using System.Collections.Generic;
using UnityEngine;
using static VoxelCraft.World.Block.BlockManager;

namespace VoxelCraft.World.Block
{
    [Serializable]
    public class Block
    {
        public Dictionary<Dir, string> UVs = new Dictionary<Dir, string>();

        public List<BlockTexture> blockTextures;

        [HideInInspector]
        public List<Texture2D> textures = new List<Texture2D>();
        [HideInInspector]
        public int id;
        [HideInInspector]
        public BlockPos pos = new BlockPos(0, 0, 0);

        public string name;

        public bool unbreakable = false;
        public bool isSolid = true;
        public float blastResistance = 1.0f;
        public float hardness = 1.0f;

        public void setupBlock(int id)
        {
            this.id = id;

            foreach(BlockTexture bt in blockTextures)
            {
                textures.Add(bt.texture);
                UVs.Add(bt.face, bt.texture.name);
            }
        }
    }

    [Serializable]
    public class BlockTexture
    {
        public Texture2D texture;
        public Dir face;
    }
}
