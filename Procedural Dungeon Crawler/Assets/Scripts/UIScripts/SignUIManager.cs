using UnityEngine;
using TMPro;
using PlayerScripts;

namespace UIScripts
{
    public class SignUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject signPanel;
        [SerializeField] private TextMeshProUGUI signText;

        private Player player;
        private bool signOpen;
        
        void Start()
        {
            signPanel.SetActive(false);
        }

        void Update()
        {
            if (!signOpen)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseSign();
            }
        }

        public void OpenSign(Player ply)
        {
            if (signOpen)
            {
                return;
            }
            
            player = ply;
            player.LockControls();
            
            signOpen = true;
            signText.text = "There is a hidden secret along the walls of this room...\n\nPress ESC to exit...";
            signPanel.SetActive(true);
        }

        public void CloseSign()
        {
            if (!signOpen)
            {
                return;
            }

            signOpen = false;
            signPanel.SetActive(false);
            
            player.UnlockControls();
        }
    }
}