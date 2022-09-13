using MoonSharp.Interpreter;

namespace VoxelCraft.Mods
{
    [MoonSharpUserData]
    public class Mod
    {
        public string name = "";
        public string description = "";
        public string version = "";
        public string creator = "";
        public string credits = "";
        public string modFolderName = "";

        public Mod(string name, string description, string version, string creator, string credits, string modFolderName)
        {
            this.name = name;
            this.description = description;
            this.version = version;
            this.creator = creator;
            this.credits = credits;
            this.modFolderName = modFolderName;
        }
    }
}
