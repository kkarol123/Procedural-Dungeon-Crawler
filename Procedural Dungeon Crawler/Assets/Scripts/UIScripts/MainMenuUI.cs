using GameManagerScripts;
using UnityEngine;

namespace UIScripts
{
    public class MainMenuUI : MonoBehaviour
    {
        public void StartGame()
        {
            ScreenFadeManager.Instance.FadeLoadScene("Dungeon");
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}