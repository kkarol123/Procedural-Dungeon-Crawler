using UnityEngine;
using UnityEngine.UI;
using PlayerScripts;
using TMPro;

namespace UIScripts
{
    public class UIScript : MonoBehaviour
    {
        [SerializeField] private Player player;
    
        [SerializeField] private Image[] hearts;
        [SerializeField] private Sprite fullHeartSprite;
    
        [SerializeField] private TMP_Text ammoText;
        
        [SerializeField] private Image keyImage;
        [SerializeField] private Sprite keySprite;
        [SerializeField] private Sprite emptyKeySprite;

        void Update()
        {
            UpdateHealth();
            UpdateAmmo();
            UpdateKey();
        }

        private void UpdateHealth()
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].enabled = i < player.Health;
            }
        }

        private void UpdateAmmo()
        {
            if (player.IsReloading)
            {
                ammoText.text = "RELOADING... / " + player.AmmoInReserve ;
            }
            else
            {
                ammoText.text = player.AmmoInClip + " / " + player.AmmoInReserve;
            }
        }

        private void UpdateKey()
        {
            if (player.HasKey)
            {
                keyImage.sprite = keySprite;
            }
            else
            {
                keyImage.sprite = emptyKeySprite;
            }
        }
    }
}