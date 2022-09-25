using System.Collections.Generic;
using UnityEngine;
using VoxelCraft.Textures;

namespace VoxelCraft.World.Block
{
	public class BlockManager : MonoBehaviour
	{
        [HideInInspector]
		public Texture2D atlas;

		public Dictionary<int, Block> blockList = new Dictionary<int, Block>();
		public static Dictionary<int, Block> staticBlockList = new Dictionary<int, Block>();

		public List<Block> blocks = new List<Block>();

		public Dictionary<string, Vector2> textureUVs = new Dictionary<string, Vector2>();

		private int lastBlockId = 0;

		public static Dictionary<string, Color> Colors = new Dictionary<string, Color>();

		/// <summary>Unit length of a cube in the texture atlas.</summary>
		public float tUnit = 0.125f;
		/// <summary>Directions.</summary>
		public enum Dir { Up, Down, North, South, East, West, All };

		void Start()
		{
			Colors["Tropical_1"] = new Color(67f / 255f, 146f / 255f, 42f / 255f);
			Colors["Tropical_2"] = new Color(51f / 255f, 112f / 255f, 32f / 255f);
			Colors["Tropical_3"] = new Color(44f / 255f, 95f / 255f, 27f / 255f);
			Colors["Tropical_4"] = new Color(31f / 255f, 69f / 255f, 20f / 255f);

			Colors["Normal_1"] = new Color(82f / 255f, 149f / 255f, 47f / 255f);
			Colors["Normal_2"] = new Color(64f / 255f, 116f / 255f, 37f / 255f);
			Colors["Normal_3"] = new Color(58f / 255f, 106f / 255f, 34f / 255f);
			Colors["Normal_4"] = new Color(53f / 255f, 97f / 255f, 31f / 255f);

			Colors["Temperate_1"] = new Color(85f / 255f, 138f / 255f, 65f / 255f);
			Colors["Temperate_2"] = new Color(77f / 255f, 126f / 255f, 60f / 255f);
			Colors["Temperate_3"] = new Color(62f / 255f, 102f / 255f, 48f / 255f);
			Colors["Temperate_4"] = new Color(56f / 255f, 90f / 255f, 43f / 255f);

			Colors["Chaparral_1"] = new Color(106f / 255f, 143f / 255f, 63f / 255f);
			Colors["Chaparral_2"] = new Color(85f / 255f, 114f / 255f, 50f / 255f);
			Colors["Chaparral_3"] = new Color(68f / 255f, 92f / 255f, 40f / 255f);
			Colors["Chaparral_4"] = new Color(56f / 255f, 76f / 255f, 34f / 255f);

			Colors["Savanna_1"] = new Color(123f / 255f, 121f / 255f, 60f / 255f);
			Colors["Savanna_2"] = new Color(104f / 255f, 103f / 255f, 51f / 255f);
			Colors["Savanna_3"] = new Color(94f / 255f, 93f / 255f, 46f / 255f);
			Colors["Savanna_4"] = new Color(74f / 255f, 73f / 255f, 36f / 255f);

			Colors["Tundra_1"] = new Color(110f / 255f, 141f / 255f, 86f / 255f);
			Colors["Tundra_2"] = new Color(94f / 255f, 121f / 255f, 73f / 255f);
			Colors["Tundra_3"] = new Color(78f / 255f, 100f / 255f, 61f / 255f);
			Colors["Tundra_4"] = new Color(58f / 255f, 74f / 255f, 45f / 255f);

			foreach (Block block in blocks)
            {
				AddBlock(block);
            }

			staticBlockList = blockList;

			Atlas at = BlockAtlasCreator.generateAtlas(blockList);

			tUnit = at.tUnit;
			atlas = at.atlas;
			textureUVs = at.uvDictionary;
		}

		public Block getBlockByName(string name)
        {
			foreach(KeyValuePair<int, Block> block in blockList)
            {
				if (block.Value.name == name)
					return block.Value;
            }

			return null;
		}

		public Block getBlockById(int id)
        {
			if (blockList.ContainsKey(id))
				return blockList.GetValueOrDefault(id);
			else
				return null;
        }

		public static Block staticGetBlockByName(string name)
		{
			foreach (KeyValuePair<int, Block> block in staticBlockList)
			{
				if (block.Value.name == name)
					return block.Value;
			}

			return null;
		}

		public static Block staticGetBlockById(int id)
		{
			if (staticBlockList.ContainsKey(id))
				return staticBlockList.GetValueOrDefault(id);
			else
				return null;
		}

		public void AddBlock(Block block)
        {
			block.setupBlock(lastBlockId);

			blockList.Add(lastBlockId, block);

			lastBlockId++;
        }

		public void regenerateBlockAtlas()
        {
			Atlas at = BlockAtlasCreator.generateAtlas(blockList);

			tUnit = at.tUnit;
			atlas = at.atlas;
			textureUVs = at.uvDictionary;
		}

		/// <summary>
		/// Gets the texure UV coordinate for a given block ID.
		/// </summary>
		/// <param name="id">BLock ID.</param>
		/// <param name="dir">Block face.</param>
		/// <returns>UV coordinate.</returns>
		public Vector2 GetTexture(int id, Dir dir)
		{
			Block block;

			blockList.TryGetValue(id, out block);

			Vector2 uv;

			if (block.UVs.ContainsKey(dir))
				textureUVs.TryGetValue(block.UVs.GetValueOrDefault(dir), out uv);
			else if (block.UVs.ContainsKey(Dir.All))
				textureUVs.TryGetValue(block.UVs.GetValueOrDefault(Dir.All), out uv);
			else
				uv = new Vector2(0, 0);

			return uv;
		}
	}
}