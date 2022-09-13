using UnityEngine;
using VoxelCraft.World;
using UnityEngine.UI;
using TMPro;

namespace VoxelCraft.UI.Screens
{
    public class CreateWorldScreen : MonoBehaviour
    {
        public TMP_InputField worldName;
        public TMP_InputField worldSeed;
        public Button nextDiffButton;
        public Button prevDiffButton;
        public TMP_Text difficultyText;
        public Toggle cheatsToggle;
        public Button createWorldButton;

        public GameObject hardcoreImage;
        public GameObject hardcoreImageFade;
        public GameObject survivalImage;
        public GameObject survivalImageFade;
        public GameObject creativeImage;
        public GameObject creativeImageFade;

        public GameObject flatWorldImage;
        public GameObject flatWorldImageFade;
        public GameObject normalWorldImage;
        public GameObject normalWorldImageFade;
        public GameObject voidWorldImage;
        public GameObject voidWorldImageFade;

        private WorldSettings worldSettings;

        private Difficulty difficulty;

        public void OpenCreateWorldScreen()
        {
            worldSettings = new WorldSettings();

            difficulty = Difficulty.NORMAL;

            difficultyText.text = string.Concat("Difficulty: " + worldSettings.difficultyToString(difficulty));
        }

        public void Start()
        {
            worldSettings = new WorldSettings();

            difficulty = Difficulty.NORMAL;

            difficultyText.text = string.Concat("Difficulty: " + worldSettings.difficultyToString(difficulty));
        }

        public void createWorld()
        {
            worldSettings.worldName = worldName.text;
            worldSettings.worldVersion = Application.version;
            worldSettings.worldSeed = worldSeed.text.GetHashCode();
            worldSettings.cheatsEnabled = cheatsToggle.isOn;
            worldSettings.worldType = getWorldType();
            worldSettings.worldGamemode = getGamemode();
            worldSettings.difficulty = difficulty;
        }

        public void nextDifficulty()
        {
            if (difficulty < Difficulty.HELL)
            {
                difficulty++;

                difficultyText.text = string.Concat("Difficulty: " + worldSettings.difficultyToString(difficulty));

                prevDiffButton.interactable = true;
            }
            else
            {
                nextDiffButton.interactable = false;
            }
        }

        public void prevDifficulty()
        {
            if (difficulty > Difficulty.PEACEFUL)
            {
                difficulty--;

                difficultyText.text = string.Concat("Difficulty: " + worldSettings.difficultyToString(difficulty));

                nextDiffButton.interactable = true;
            }
            else
            {
                prevDiffButton.interactable = false;
            }
        }

        public void survivalImgPressed()
        {

        }

        public void creativeImgPressed()
        {

        }

        public void hardcoreImgPressed()
        {

        }

        public void normalWorldImgPressed()
        {

        }

        public void flatWorldImgPressed()
        {

        }

        public void voidWorldImgPressed()
        {

        }

        public WorldType getWorldType()
        {
            if (normalWorldImage.GetComponent<Toggle>().isOn)
            {
                return WorldType.NORMAL;
            } 
            else if (flatWorldImage.GetComponent<Toggle>().isOn)
            {
                return WorldType.FLAT;
            } 
            else if (voidWorldImage.GetComponent<Toggle>().isOn)
            {
                return WorldType.VOID;
            } 
            else
            {
                return WorldType.NORMAL;
            }
        }

        public Gamemode getGamemode()
        {
            if (survivalImage.GetComponent<Toggle>().isOn)
            {
                return Gamemode.SURVIVAL;
            }
            else if (creativeImage.GetComponent<Toggle>().isOn)
            {
                return Gamemode.CREATIVE;
            }
            else if (hardcoreImage.GetComponent<Toggle>().isOn)
            {
                return Gamemode.HARDCORE;
            }
            else
            {
                return Gamemode.SURVIVAL;
            }
        }
    }
}
