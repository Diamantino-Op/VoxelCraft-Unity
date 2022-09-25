using UnityEngine;

namespace VoxelCraft.UI.Screens
{
    public class MainMenuScreen : MonoBehaviour
    {
        private GameManager gameManager;
        private GUIManager guiManager;

        public GameObject mainMenuScreen;

        public void openMainMenuScreen(GameManager gameManager, GUIManager guiManager)
        {
            this.gameManager = gameManager;
            this.guiManager = guiManager;

            mainMenuScreen.SetActive(true);
        }

        public void openWorldListScreen()
        {
            Debug.Log("SinglePlayer Button");
            guiManager.worldListScreen.openWorldListScreen(gameManager, guiManager);

            mainMenuScreen.SetActive(false);
        }

        public void openServerListScreen()
        {
            Debug.Log("MultiPlayer Button");
        }

        public void openOptionsScreen()
        {
            Debug.Log("Options Button");
        }

        public void quitButton()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
