using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.IO;
using UnityEngine;
using VoxelCraft.Mods;

namespace VoxelCraft.API
{
    public class VoxelCraftAPI : MonoBehaviour
	{
		public ModManager modManager;
        [HideInInspector]
		public string modPath;

        public void Start()
        {
			modPath = Application.persistentDataPath + "/Mods";

			if (!Directory.Exists(modPath))
				Directory.CreateDirectory(modPath);

			string[] mods = Directory.GetDirectories(modPath);

			foreach(string mod in mods)
            {
				string modFolderName = mod.Replace(modPath + @"\", "");

				Script script = new Script();
				script.Options.ScriptLoader = new FileSystemScriptLoader();

				script.DoFile(mod + "/Main.lua");

				DynValue name = script.Call(script.Globals["getName"]);
				DynValue description = script.Call(script.Globals["getDescription"]);
				DynValue version = script.Call(script.Globals["getVersion"]);
				DynValue creator = script.Call(script.Globals["getCreator"]);
				DynValue credits = script.Call(script.Globals["getCredits"]);

				modManager.addMod(new Mod(name.String, description.String, version.String, creator.String, credits.String, modFolderName));
			}

		}
	}
}
