using UnityEngine;
using VoxelCraft.API;
using VoxelCraft.Mods;
using VoxelCraft.World.Block;

namespace VoxelCraft
{
    public class GameManager : MonoBehaviour
    {
        public BlockManager blockManager;
        public ModManager modManager;
        public VoxelCraftAPI api;

        [HideInInspector]
        public string worldName = "World";

        public void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
