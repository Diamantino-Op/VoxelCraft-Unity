using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelCraft.UI
{
    public class GUIManager : MonoBehaviour
    {
        [HideInInspector]
        public GameManager gameManager;

        public GameObject menuPanel;
        public GameObject settingsPanel;
        public GameObject pausePanel;
        public GameObject worldSelectionPanel;
        public GameObject worldCreationPanel;
        public GameObject worldSettingsPanel;
        public GameObject joiningWorldPanel;
        public GameObject serverListPanel;
        public GameObject addServerPanel;
        public GameObject joiningServerPanel;

        public void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            DontDestroyOnLoad(gameObject);
        }

        public void CreateWorld()
        {

        }
    }
}
