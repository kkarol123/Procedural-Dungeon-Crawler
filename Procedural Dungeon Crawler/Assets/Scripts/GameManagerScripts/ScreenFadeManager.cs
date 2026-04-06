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
        [SerializeField] private Canvas fadeCanvas;
        [SerializeField] private float fadeSpeed = 2f;
        
        private GameManager gameManager;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;

            Color color = blackScreen.color;
            color.a = 0f;
            blackScreen.color = color;
            
            gameManager = FindFirstObjectByType<GameManager>();
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            fadeCanvas.sortingOrder = 999;
        }

        
        
        
        public void FadeTeleport(Player player, Vector3 targetPosition)
        {
            StartCoroutine(FadeTeleportCoroutine(player, targetPosition));
        }
        public void FadeLoadScene(string sceneName)
        {
            StartCoroutine(FadeLoadSceneCoroutine(sceneName));
        }

        public void FadeNextFloor()
        {
            StartCoroutine(FadeNextFloorCoroutine());
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
            yield return StartCoroutine(FadeIn());
            
            SceneManager.LoadScene(sceneName);
            fadeCanvas.sortingOrder = 999;
            
            yield return new WaitForSeconds(1f); //wait a bit for the scene to fully load
            yield return StartCoroutine(FadeOut());
        }
        private IEnumerator FadeNextFloorCoroutine()
        {
            yield return StartCoroutine(FadeIn());
            gameManager.LoadNextFloor();
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