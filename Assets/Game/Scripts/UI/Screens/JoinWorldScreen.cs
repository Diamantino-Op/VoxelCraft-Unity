using UnityEngine;

namespace VoxelCraft.UI.Screens
{
    public class JoinWorldScreen : MonoBehaviour
    {
        private GameManager gameManager;
        private GUIManager guiManager;

        public GameObject joinWorldScreen;

        public void openJoinWorldScreen(GameManager gameManager, GUIManager guiManager)
        {
            this.gameManager = gameManager;
            this.guiManager = guiManager;

            joinWorldScreen.SetActive(true);
        }
    }
}
