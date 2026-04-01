using UnityEngine;
using TMPro;
using PlayerScripts;

namespace UIScripts
{
    public class ChestRewardManager : MonoBehaviour
    {
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private TextMeshProUGUI rewardText;

        private Player currentPlayer;
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
        }

        public void OpenChestReward(Player player)
        {
            //implement
        }
    }
}