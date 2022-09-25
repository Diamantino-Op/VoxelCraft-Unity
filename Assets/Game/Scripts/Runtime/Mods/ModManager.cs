using System.Collections.Generic;
using UnityEngine;

namespace VoxelCraft.Mods
{
    public class ModManager : MonoBehaviour
    {
        public List<Mod> modList = new List<Mod>();

        public void addMod(Mod mod)
        {
            modList.Add(mod);

            Debug.Log("Name: " + mod.name);
            Debug.Log("Description: " + mod.description);
            Debug.Log("Version: " + mod.version);
            Debug.Log("Creator: " + mod.creator);
            Debug.Log("Credits: " + mod.credits);
            Debug.Log("ModFolderName: " + mod.modFolderName);
        }
    }
}
