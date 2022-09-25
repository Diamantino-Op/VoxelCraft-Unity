using System.Collections;
using System.Collections.Generic;
using VoxelCraft.UI.Screens;
using UnityEngine;

namespace VoxelCraft.UI
{
    public class GUIManager : MonoBehaviour
    {
        [HideInInspector]
        public GameManager gameManager;

        public CreateWorldScreen createWorldScreen;
        public WorldListScreen worldListScreen;
        public MainMenuScreen mainMenuScreen;
        public JoinWorldScreen joinWorldScreen;

        public void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            openMainMenuScreen();

            DontDestroyOnLoad(gameObject);
        }

        public void openMainMenuScreen()
        {
            mainMenuScreen.openMainMenuScreen(gameManager, this);
        }
    }
}
