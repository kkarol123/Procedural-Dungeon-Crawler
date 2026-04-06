using UnityEngine;
using System.Collections;
using DungeonLayoutGeneration.Settings;
using GameManagerScripts;
using UnityEngine.Tilemaps;

namespace PlayerScripts
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 20f;
        private bool controlsLocked;

        [SerializeField] private int damage = 1;
        [SerializeField] private Transform gunTip;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] int clipSize = 8;
        private int ammoInClip = 8;
        private int ammoInReserve = 16;
        [SerializeField] int maxAmmo = 32;
        private bool isReloading;
        [SerializeField] private float reloadTime = 2f;
        
        [SerializeField] private int health = 5;
        [SerializeField] private int maxHealth = 5;
        private bool canTakeDamage = true;
        [SerializeField] private Color damageColor = Color.red;
        private Color originalColor;
        [SerializeField] private Sprite playerDeadSprite;

        [SerializeField] private bool hasKey = true;
        
        [SerializeField] private Tilemap floorsTilemap;
        [SerializeField] private DungeonSettings dungeonSettings;
        [SerializeField] private float waterSpeedMultiplier = 0.5f;
        [SerializeField] private float lavaSpeedMultiplier = 0.8f;
        private float currentWalkSpeed;
        
        
        
        public int Health => health;
        public int AmmoInClip => ammoInClip;
        public int AmmoInReserve => ammoInReserve;
        public bool IsReloading => isReloading;
        public bool HasKey => hasKey;
        public int MaxHealth => maxHealth;
        public int MaxAmmo => maxAmmo;
        public int Damage => damage;
        public bool ControlsLocked => controlsLocked;
        
        
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        void Update()
        {
            CheckForEnvironmentEffects();
            
            PlayerMovement();
            PlayerRotate();
            
            PlayerShoot();
            CheckIfPlayerReloading();
        }

        //Player movement and rotation
        private void PlayerMovement()
        {
            if (controlsLocked)
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

            rb.linearVelocity = new Vector2(moveX, moveY).normalized * currentWalkSpeed;   //normalized will fix movement speed diagonally being faster than vertical movement speed
        }

        private void PlayerRotate()
        {
            if (controlsLocked)
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
            if (isReloading || controlsLocked)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (ammoInClip > 0)
                {
                    ammoInClip--;
                    GameObject bulletObject = Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
                    BulletScript bulletScript = bulletObject.GetComponent<BulletScript>();
                    bulletScript.Initialise(damage);
                }
            }
        }

        private void CheckIfPlayerReloading()
        {
            if (controlsLocked)
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

        public void AddAmmoInReserve()
        {
            int totalAmmo = ammoInClip + ammoInReserve;
            totalAmmo += 8;

            if (totalAmmo > maxAmmo)
            {
                totalAmmo = maxAmmo;
            }

            ammoInReserve = totalAmmo - ammoInClip;
        }
        
        
        //Player health
        public void PlayerTakeDamage()
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

        private IEnumerator DamageCoolDownCoroutine()
        {
            canTakeDamage = false;

            spriteRenderer.color = damageColor;
            
            yield return new WaitForSeconds(2f);

            spriteRenderer.color = originalColor;
            canTakeDamage = true;
        }

        public void PlayerGetHealth()
        {
            if (controlsLocked)
            {
                return;
            }
            
            if (health < maxHealth)
            {
                health++;
            }
        }

        private void PlayerDied()
        {
            if (controlsLocked)
            {
                return;
            }

            rb.mass = 100000;   //to not allow the enemies from moving the player
            spriteRenderer.sprite = playerDeadSprite;
            LockControls();

            StartCoroutine(LoadDeathScene());
        }

        private IEnumerator LoadDeathScene()
        {
            yield return new WaitForSeconds(3f);
            ScreenFadeManager.Instance.FadeLoadScene("DeathScene");
        }
        
        
        
        //Player key
        public void GiveKey()
        {
            hasKey = true;
        }
        
        public void UseKey()
        {
            hasKey = false;
        }
        
        
        
        //Environment Effects
        private void CheckForEnvironmentEffects()
        {
            currentWalkSpeed = moveSpeed;
            
            Vector3Int playerCellPosition = floorsTilemap.WorldToCell(transform.position);
            TileBase currentTile = floorsTilemap.GetTile(playerCellPosition);

            if (currentTile == dungeonSettings.PuddleTile)
            {
                currentWalkSpeed = moveSpeed * waterSpeedMultiplier;
            }

            if (currentTile == dungeonSettings.LavaTile)
            {
                currentWalkSpeed = moveSpeed * lavaSpeedMultiplier;
                PlayerTakeDamage();
            }
        }
        
        
        
        //Lock player
        public void LockControls()
        {
            controlsLocked = true;
            rb.linearVelocity = Vector3.zero;
            isReloading = false;
        }

        public void UnlockControls()
        {
            controlsLocked = false;
        }
        
        
        
        //Upgrades
        public void IncreaseMaxAmmo()
        {
            maxAmmo += 8;
            ammoInReserve += 8;
            
            int totalAmmo = ammoInClip + ammoInReserve;
            if (totalAmmo > maxAmmo)
            {
                ammoInReserve = maxAmmo - ammoInClip;
            }
        }

        public void IncreaseMaxHealth()
        {
            maxHealth += 1;
            health += 1;
        }

        public void IncreaseDamage()
        {
            damage += 1;
        }
    }
}