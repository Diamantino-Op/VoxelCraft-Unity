using UnityEngine.UI;
using System.IO;
using UnityEngine;
using TMPro;

namespace VoxelCraft.UI.Screens
{
    public class WorldListScreen : MonoBehaviour
    {
        private GameManager gameManager;
        private GUIManager guiManager;

        public GameObject worldViewPrefab;
        public Transform worldList;

        public GameObject worldListScreen;
        public GameObject worldViewContainer;

        public void openWorldListScreen(GameManager gameManager, GUIManager guiManager)
        {
            this.gameManager = gameManager;
            this.guiManager = guiManager;

            createWorldList();

            worldListScreen.SetActive(true);
        }

        public void openCreateWorldScreen()
        {
            guiManager.createWorldScreen.openCreateWorldScreen(gameManager, guiManager);

            worldListScreen.SetActive(false);
        }

        public void cancelButton()
        {
            guiManager.mainMenuScreen.openMainMenuScreen(gameManager, guiManager);

            worldListScreen.SetActive(false);
        }

        public void createWorldList()
        {
            string[] worlds = Directory.GetDirectories(string.Concat(Application.persistentDataPath, "/Worlds/"));

            foreach (string world in worlds)
            {
                GameObject wvp = Instantiate(worldViewPrefab);

                wvp.transform.SetParent(worldList);

                if (File.Exists(string.Concat(world, "/worldIcon.png")))
                {
                    Texture2D worldIcon = new Texture2D(2, 2);

                    worldIcon.LoadImage(File.ReadAllBytes(string.Concat(world, "/worldIcon.png")));

                    wvp.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(worldIcon, new Rect(0, 0, worldIcon.width, worldIcon.height), new Vector2(0.5f, 0.5f));
                }

                string worldName = "Error";
                string worldVersion = "Error";
                string creationDate = "Error";

                string[] worldFile = File.ReadAllLines(string.Concat(world, "/config.conf"));

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
                        case "Creation Date":
                            creationDate = settingValue;
                            break;
                    }
                }

                wvp.transform.GetChild(1).GetComponent<TMP_Text>().text = worldName;
                wvp.transform.GetChild(2).GetComponent<TMP_Text>().text = "Version: " + worldVersion + " Creation Date: " + creationDate;

                wvp.GetComponent<PlayWorldScreen>().setupButton(gameManager, guiManager, worldName);
            }
        }
    }
}
