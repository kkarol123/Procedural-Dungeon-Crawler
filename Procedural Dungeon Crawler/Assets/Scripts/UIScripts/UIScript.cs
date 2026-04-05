using GameManagerScripts;
using UnityEngine;
using UnityEngine.UI;
using PlayerScripts;
using TMPro;

namespace UIScripts
{
    public class UIScript : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private GameManager gameManager;
    
        [SerializeField] private Image[] hearts;
        [SerializeField] private Sprite fullHeartSprite;
    
        [SerializeField] private TMP_Text ammoText;
        
        [SerializeField] private Image keyImage;
        [SerializeField] private Sprite keySprite;
        [SerializeField] private Sprite emptyKeySprite;
        
        [SerializeField] private TMP_Text damageText;

        [SerializeField] private TMP_Text floorText;

        void Update()
        {
            UpdateHealth();
            UpdateAmmo();
            UpdateKey();
            UpdateDamage();
            UpdateFloor();
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

        private void UpdateDamage()
        {
            damageText.text = "DMG: " + player.Damage;
        }

        private void UpdateFloor()
        {
            floorText.text = "FLOOR: " + gameManager.FloorNumber;
        }
    }
}