using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 20f;
        
        [SerializeField] private Transform gunTip;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] int clipSize = 8;
        private int ammoInClip = 8;
        private int ammoInReserve = 16;
        private int maxAmmo = 32;
        private bool isReloading;
        [SerializeField] float reloadTime = 2f;
        
        [SerializeField] private int health = 5;
        [SerializeField] private int maxHealth = 5;
        private bool canTakeDamage = true;
        [SerializeField] private Color damageColor = Color.red;
        private Color originalColor;
        private bool playerDied;
        [SerializeField] private Sprite playerDeadSprite;

        [SerializeField] bool hasKey = true;
        
        public int Health => health;
        public int AmmoInClip => ammoInClip;
        public int AmmoInReserve => ammoInReserve;
        public bool IsReloading => isReloading;
        public bool HasKey => hasKey;
        
        
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        void Update()
        {
            PlayerMovement();
            PlayerRotate();
            PlayerShoot();
            CheckIfPlayerReloading();
            
            PlayerGetHealth();
            PlayerTakeDamage();
        }

        //Player movement and rotation
        private void PlayerMovement()
        {
            if (playerDied)
            {
                return;
            }
            
            float moveX = 0f;
            float moveY = 0f;
            
            if (Input.GetKey(KeyCode.W))
            {
                moveY = 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveX = -1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveY = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveX = 1f;
            }

            rb.linearVelocity = new Vector2(moveX, moveY).normalized * moveSpeed;   //normalized will fix movement speed diagonally being faster than vertical movement speed
        }

        private void PlayerRotate()
        {
            if (playerDied)
            {
                return;
            }
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector2 direction = mousePos - transform.position;
            if (direction.sqrMagnitude < 0.1f)    //if the mouse is too close to the player, don't rotate
            {
                return;
            }
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        

        
        //Player shooting
        private void PlayerShoot()
        {
            if (isReloading)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (ammoInClip > 0)
                {
                    ammoInClip--;
                    Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
                }
            }
        }

        private void CheckIfPlayerReloading()
        {
            if (playerDied)
            {
                return;
            }
            if (isReloading)
            {
                return;
            }
            if (ammoInClip == clipSize)
            {
                return;
            }
            if (ammoInReserve <= 0)
            {
                return;
            }
            
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
        
        private IEnumerator ReloadCoroutine()
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);

            int ammoNeeded = clipSize - ammoInClip;
            int ammoToReload = Mathf.Min(ammoNeeded, ammoInReserve);

            ammoInClip += ammoToReload;
            ammoInReserve -= ammoToReload;

            isReloading = false;
        }
        
        
        //Player health
        private void PlayerTakeDamage()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (!canTakeDamage)
                {
                    return;
                }
                
                health--;
                if (health <= 0)
                {
                    PlayerDied();
                    return;
                }

                StartCoroutine(DamageCoolDownCoroutine());
            }
        }

        private IEnumerator DamageCoolDownCoroutine()
        {
            canTakeDamage = false;

            spriteRenderer.color = damageColor;
            
            yield return new WaitForSeconds(2f);

            spriteRenderer.color = originalColor;
            canTakeDamage = true;
        }

        private void PlayerGetHealth()
        {
            if (playerDied)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (health < maxHealth)
                {
                    health++;
                }
            }
        }

        private void PlayerDied()
        {
            if (playerDied)
            {
                return;
            }
            
            spriteRenderer.sprite = playerDeadSprite;
            playerDied = true;
            isReloading = false;

            StartCoroutine(CooldownToDiedScene());
        }

        private IEnumerator CooldownToDiedScene()
        {
            yield return new WaitForSeconds(3f);
            
            SceneManager.LoadScene("DeathScene");
        }
    }
}