using GameManagerScripts;
using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class DeathSceneUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text floorText;
        void Start()
        {
            floorText.text = "YOU MADE IT " + RunData.FloorsReached + " FLOORS!";
        }
        
        public void ReturnToMainMenu()
        {
            ScreenFadeManager.Instance.FadeLoadScene("MainMenu");
        }
    }
}