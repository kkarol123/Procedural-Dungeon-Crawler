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
                "Choose a reward\n\n" +
                "1 - Increase Max Ammo \n" +
                "2 - Increase Max Health \n" +
                "3 - Increase Damage";
        }

        private void CloseReward()
        {
            rewardPanel.SetActive(false);
            player.UnlockControls();
            rewardActive = false;
        }
    }
}