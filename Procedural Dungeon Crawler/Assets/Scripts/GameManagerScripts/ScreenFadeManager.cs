using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PlayerScripts;
using UnityEngine.SceneManagement;

namespace GameManagerScripts
{
    public class ScreenFadeManager : MonoBehaviour
    {
        public static ScreenFadeManager Instance { get; private set; }

        [SerializeField] private Image blackScreen;
        [SerializeField] private float fadeSpeed = 2f;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Color color = blackScreen.color;
            color.a = 0f;
            blackScreen.color = color;
        }

        
        
        
        public void FadeTeleport(Player player, Vector3 targetPosition)
        {
            StartCoroutine(FadeTeleportCoroutine(player, targetPosition));
        }
        public void FadeLoadScene(string sceneName)
        {
            StartCoroutine(FadeLoadSceneCoroutine(sceneName));
        }
            

            
        
        private IEnumerator FadeTeleportCoroutine(Player player, Vector3 targetPosition)
        {
            yield return StartCoroutine(FadeIn());
            
            player.transform.position = targetPosition;
            CameraFollowPlayer cameraFollowPlayer = FindFirstObjectByType<CameraFollowPlayer>();
            cameraFollowPlayer.SnapToPlayer();
            
            yield return StartCoroutine(FadeOut());
        }
        private IEnumerator FadeLoadSceneCoroutine(string sceneName)
        {
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(FadeIn());
            SceneManager.LoadScene(sceneName);
            yield return null; //wait a frame for the scene to load
            yield return StartCoroutine(FadeOut());
        }

        
        
        
        
        private IEnumerator FadeIn()
        {
            Color color = blackScreen.color;

            while (color.a < 1f)
            {
                color.a += Time.deltaTime * fadeSpeed;
                blackScreen.color = color;
                yield return null;   //wait one frame
            }
        }

        private IEnumerator FadeOut()
        {
            Color color = blackScreen.color;

            while (color.a > 0f)
            {
                color.a -= Time.deltaTime * fadeSpeed;
                blackScreen.color = color;
                yield return null;  //wait one frame
            }
        }
    }
}