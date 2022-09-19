using UnityEngine;
using VoxelCraft.World;
using UnityEngine.UI;
using TMPro;
using Mirror;

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
        public Toggle survivalToggle;
        public GameObject creativeImage;
        public GameObject creativeImageFade;

        public GameObject flatWorldImage;
        public GameObject flatWorldImageFade;
        public GameObject normalWorldImage;
        public GameObject normalWorldImageFade;
        public Toggle normalWorldToggle;
        public GameObject voidWorldImage;
        public GameObject voidWorldImageFade;

        private WorldSettings worldSettings;
        private GameManager gameManager;
        private GUIManager guiManager;

        public GameObject createWorldScreen;

        private Difficulty difficulty;

        public void openCreateWorldScreen(GameManager gameManager, GUIManager guiManager)
        {
            this.gameManager = gameManager;
            this.guiManager = guiManager;
            worldSettings = new WorldSettings();

            difficulty = Difficulty.NORMAL;

            difficultyText.text = string.Concat("Difficulty: " + worldSettings.difficultyToString(difficulty));

            survivalToggle.isOn = true;
            survivalImgPressed();
            normalWorldToggle.isOn = true;
            normalWorldImgPressed();

            createWorldScreen.SetActive(true);
        }

        public void createWorld()
        {
            if (worldName.text != "")
                worldSettings.worldName = worldName.text;
            else
                worldSettings.worldName = "World";
            worldSettings.worldVersion = Application.version;
            worldSettings.worldSeed = worldSeed.text.GetHashCode();
            worldSettings.cheatsEnabled = cheatsToggle.isOn;
            worldSettings.worldType = getWorldType();
            worldSettings.worldGamemode = getGamemode();
            worldSettings.difficulty = difficulty;

            worldSettings.saveToFile(string.Concat(Application.persistentDataPath, "/Worlds/", worldName.text, "/"));

            gameManager.worldName = worldName.text;

            guiManager.joinWorldScreen.openJoinWorldScreen(gameManager, guiManager);

            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().StartHost();

            createWorldScreen.SetActive(false);
        }

        public void cancelButton()
        {
            guiManager.worldListScreen.openWorldListScreen(gameManager, guiManager);

            createWorldScreen.SetActive(false);
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
            survivalImage.transform.localScale = new Vector3(1f, 1f, 1f);
            survivalImageFade.SetActive(false);

            creativeImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            creativeImageFade.SetActive(true);

            hardcoreImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            hardcoreImageFade.SetActive(true);
        }

        public void creativeImgPressed()
        {
            survivalImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            survivalImageFade.SetActive(true);

            creativeImage.transform.localScale = new Vector3(1f, 1f, 1f);
            creativeImageFade.SetActive(false);

            hardcoreImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            hardcoreImageFade.SetActive(true);
        }

        public void hardcoreImgPressed()
        {
            survivalImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            survivalImageFade.SetActive(true);

            creativeImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            creativeImageFade.SetActive(true);

            hardcoreImage.transform.localScale = new Vector3(1f, 1f, 1f);
            hardcoreImageFade.SetActive(false);
        }

        public void normalWorldImgPressed()
        {
            normalWorldImage.transform.localScale = new Vector3(1f, 1f, 1f);
            normalWorldImageFade.SetActive(false);

            flatWorldImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            flatWorldImageFade.SetActive(true);

            voidWorldImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            voidWorldImageFade.SetActive(true);
        }

        public void flatWorldImgPressed()
        {
            normalWorldImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            normalWorldImageFade.SetActive(true);

            flatWorldImage.transform.localScale = new Vector3(1f, 1f, 1f);
            flatWorldImageFade.SetActive(false);

            voidWorldImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            voidWorldImageFade.SetActive(true);
        }

        public void voidWorldImgPressed()
        {
            normalWorldImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            normalWorldImageFade.SetActive(true);

            flatWorldImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            flatWorldImageFade.SetActive(true);

            voidWorldImage.transform.localScale = new Vector3(1f, 1f, 1f);
            voidWorldImageFade.SetActive(false);
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
