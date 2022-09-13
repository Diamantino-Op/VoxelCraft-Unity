
using System.Collections.Generic;
using System.IO;

namespace VoxelCraft.World
{
    public class WorldSettings
    {
        public string worldName;
        public string worldVersion;
        public int worldSeed;
        public bool cheatsEnabled;
        public WorldType worldType;
        public Gamemode worldGamemode;
        public Difficulty difficulty;
        public GameRule mobSpawning;
        public GameRule alwaysDay;
        public GameRule daylightCycle;
        public GameRule weatherCycle;
        public GameRule gameSpeed;

        public WorldSettings(string worldFilePath)
        {
            loadFromFile(worldFilePath);
        }

        public WorldSettings() 
        {
            mobSpawning = new GameRule("Mob Spawning", "Enable or disable mob spawning", true);
            alwaysDay = new GameRule("Always Day", "Time will always be day", false);
            daylightCycle = new GameRule("Daylight Cycle", "Enable or disable the day/night cycle", true);
            weatherCycle = new GameRule("Weather Cycle", "Enable or disable the changing of the weather", true);
            gameSpeed = new GameRule("Game Speed", "The speed of the game", 1.0f);
        }

        public void loadFromFile(string worldFilePath)
        {
            string[] worldFile = File.ReadAllLines(worldFilePath);

            foreach (string line in worldFile)
            {
                string[] temp = line.Split(":");

                string settingName = temp[0];
                string settingValue = temp[1].Replace(" ", "");

                switch (settingName)
                {
                    case "World Name":
                        worldName = settingValue;
                        break;
                    case "World Version":
                        worldVersion = settingValue;
                        break;
                    case "World Seed":
                        worldSeed = int.Parse(settingValue);
                        break;
                    case "Cheats Enabled":
                        cheatsEnabled = bool.Parse(settingValue);
                        break;
                    case "World Type":
                        worldType = stringToWorldType(settingValue);
                        break;
                    case "World Gamemode":
                        worldGamemode = stringToGamemode(settingValue);
                        break;
                    case "Difficulty":
                        difficulty = stringToDifficulty(settingValue);
                        break;
                    case "Mob Spawning":
                        mobSpawning = new GameRule("Mob Spawning", "Enable or disable mob spawning", bool.Parse(settingValue));
                        break;
                    case "Always Day":
                        alwaysDay = new GameRule("Always Day", "Time will always be day", bool.Parse(settingValue));
                        break;
                    case "Daylight Cycle":
                        daylightCycle = new GameRule("Daylight Cycle", "Enable or disable the day/night cycle", bool.Parse(settingValue));
                        break;
                    case "Weather Cycle":
                        weatherCycle = new GameRule("Weather Cycle", "Enable or disable the changing of the weather", bool.Parse(settingValue));
                        break;
                    case "Game Speed":
                        gameSpeed = new GameRule("Game Speed", "The speed of the game", float.Parse(settingValue));
                        break;
                }
            }
        }

        public void saveToFile(string worldFilePath)
        {
            List<string> settings = new List<string>();

            settings.Add(string.Concat("World Name: ", worldName));
            settings.Add(string.Concat("World Version: ", worldVersion));
            settings.Add(string.Concat("World Seed: ", worldSeed));
            settings.Add(string.Concat("Cheats Enabled: ", cheatsEnabled));
            settings.Add(string.Concat("World Type: ", worldTypeToString(worldType)));
            settings.Add(string.Concat("World Gamemode: ", gamemodeToString(worldGamemode)));
            settings.Add(string.Concat("Difficulty: ", difficultyToString(difficulty)));
            settings.Add(string.Concat("Mob Spawning: ", mobSpawning.getValueAsString()));
            settings.Add(string.Concat("Always Day: ", alwaysDay.getValueAsString()));
            settings.Add(string.Concat("Daylight Cycle: ", daylightCycle.getValueAsString()));
            settings.Add(string.Concat("Weather Cycle: ", weatherCycle.getValueAsString()));
            settings.Add(string.Concat("Game Speed: ", gameSpeed.getValueAsString()));

            File.WriteAllLines(worldFilePath, settings.ToArray());
        }

        public Difficulty stringToDifficulty(string difficultyString)
        {
            switch(difficultyString)
            {
                case "Easy":
                    return Difficulty.EASY;
                case "Normal":
                    return Difficulty.NORMAL;
                case "Hard":
                    return Difficulty.HARD;
                case "Extreme":
                    return Difficulty.EXTREME;
                case "Nightmare":
                    return Difficulty.NIGHTMARE;
                case "Hell":
                    return Difficulty.HELL;
                default:
                    return Difficulty.PEACEFUL;
            }
        }

        public Gamemode stringToGamemode(string gamemodeString)
        {
            switch (gamemodeString)
            {
                case "Creative":
                    return Gamemode.CREATIVE;
                case "Hardcore":
                    return Gamemode.HARDCORE;
                default:
                    return Gamemode.SURVIVAL;
            }
        }

        public WorldType stringToWorldType(string worldTypeString)
        {
            switch (worldTypeString)
            {
                case "Flat":
                    return WorldType.FLAT;
                case "Void":
                    return WorldType.VOID;
                default:
                    return WorldType.NORMAL;
            }
        }

        public string difficultyToString(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.EASY:
                    return "Easy";
                case Difficulty.NORMAL:
                    return "Normal";
                case Difficulty.HARD:
                    return "Hard";
                case Difficulty.EXTREME:
                    return "Extreme";
                case Difficulty.NIGHTMARE:
                    return "Nightmare";
                case Difficulty.HELL:
                    return "Hell";
                default:
                    return "Peaceful";
            }
        }

        public string gamemodeToString(Gamemode gamemode)
        {
            switch (gamemode)
            {
                case Gamemode.CREATIVE:
                    return "Creative";
                case Gamemode.HARDCORE:
                    return "Hardcore";
                default:
                    return "Survival";
            }
        }

        public string worldTypeToString(WorldType worldType)
        {
            switch (worldType)
            {
                case WorldType.FLAT:
                    return "Flat";
                case WorldType.VOID:
                    return "Void";
                default:
                    return "Normal";
            }
        }
    }

    public class GameRule
    {
        public string name;
        public string description;
        public GameRuleType type;

        public int intValue = 0;
        public bool boolValue = false;
        public string stringValue = "";
        public float floatValue = 0.0f;

        public GameRule(string name, string description, GameRuleType type)
        {
            this.name = name;
            this.description = description;
            this.type = type;
        }

        public GameRule(string name, string description, int intValue)
        {
            this.name = name;
            this.description = description;
            this.type = GameRuleType.INTEGER;
            this.intValue = intValue;
        }

        public GameRule(string name, string description, bool boolValue)
        {
            this.name = name;
            this.description = description;
            this.type = GameRuleType.BOOLEAN;
            this.boolValue = boolValue;
        }

        public GameRule(string name, string description, string stringValue)
        {
            this.name = name;
            this.description = description;
            this.type = GameRuleType.STRING;
            this.stringValue = stringValue;
        }

        public GameRule(string name, string description, float floatValue)
        {
            this.name = name;
            this.description = description;
            this.type = GameRuleType.FLOAT;
            this.floatValue = floatValue;
        }

        public string getValueAsString()
        {
            if (intValue != 0)
                return intValue.ToString();
            else if (boolValue != false)
                return boolValue.ToString();
            else if (stringValue != "")
                return stringValue;
            else if (floatValue != 0.0f)
                return floatValue.ToString();
            else
                return "";
        }
    }

    public enum Difficulty
    {
        PEACEFUL,
        EASY,
        NORMAL,
        HARD,
        EXTREME,
        NIGHTMARE,
        HELL
    }

    public enum Gamemode
    {
        SURVIVAL,
        CREATIVE,
        HARDCORE
    }

    public enum WorldType
    {
        FLAT,
        VOID,
        NORMAL
    }

    public enum GameRuleType
    {
        INTEGER,
        BOOLEAN,
        STRING,
        FLOAT
    }
}
