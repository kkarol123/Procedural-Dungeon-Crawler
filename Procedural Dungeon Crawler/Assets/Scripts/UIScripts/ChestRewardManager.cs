using UnityEngine;
using TMPro;
using PlayerScripts;

namespace UIScripts
{
    public class ChestRewardManager : MonoBehaviour
    {
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private TextMeshProUGUI rewardText;

        private Player player;
        private bool rewardActive;
        
        private void Start()
        {
            rewardPanel.SetActive(false);
        }

        private void Update()
        {
            if (!rewardActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))   //alpha1 means number 1 on the top of the keyboard
            {
                player.IncreaseMaxAmmo();
                CloseReward();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                player.IncreaseMaxHealth();
                CloseReward();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                player.IncreaseDamage();
                CloseReward();
            }
        }

        public void OpenChestReward(Player ply)
        {
            if (rewardActive)
            {
                return;
            }

            player = ply;
            rewardActive = true;
            
            player.LockControls();
            
            rewardPanel.SetActive(true);
            rewardText.text =
                "CHOOSE A REWARD\n\n" +
                "1 - INCREASE MAX AMMO \n" +
                "2 - INCREASE MAX HEALTH \n" +
                "3 - INCREASE DAMAGE";
        }

        private void CloseReward()
        {
            rewardPanel.SetActive(false);
            player.UnlockControls();
            rewardActive = false;
        }
    }
}