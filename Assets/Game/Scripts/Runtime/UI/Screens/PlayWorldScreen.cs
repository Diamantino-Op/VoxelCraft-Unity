using UnityEngine;
using Mirror;

namespace VoxelCraft.UI.Screens
{
    public class PlayWorldScreen : MonoBehaviour
    {
        private GameManager gameManager;
        private GUIManager guiManager;

        private string worldName;

        public void setupButton(GameManager gameManager, GUIManager guiManager, string worldName)
        {
            this.gameManager = gameManager;
            this.guiManager = guiManager;
            this.worldName = worldName;
        }

        public void playWorld()
        {
            gameManager.worldName = worldName;

            guiManager.joinWorldScreen.openJoinWorldScreen(gameManager, guiManager);

            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().StartHost();

            guiManager.worldListScreen.worldListScreen.SetActive(false);
        }
    }
}
