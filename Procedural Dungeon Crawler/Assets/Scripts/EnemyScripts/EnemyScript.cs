using UnityEngine;
using PlayerScripts;
using System.Collections;

namespace EnemyScripts
{
    public abstract class EnemyScript : MonoBehaviour
    {
        [SerializeField] int maxHealth;
        private int currentHealth;
        
        protected float moveSpeed = 0.8f;

        protected float detectionRange = 6f;
        
        protected Player player;
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        [SerializeField] protected Sprite attackSprite;
        [SerializeField] protected float attackSpriteDuration = 0.15f;

        private Sprite defaultSprite;

        [SerializeField] private float damageCooldown = 0.1f;
        [SerializeField] private Color damageColor = Color.red;

        private bool canTakeDamage = true;
        private Color originalColor;
        
    
        protected virtual void Start()
        {
            currentHealth = maxHealth;
            
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();            
            animator = GetComponent<Animator>();

            defaultSprite = spriteRenderer.sprite;
            originalColor = spriteRenderer.color;
            
            player = FindFirstObjectByType<Player>();
        }

        protected virtual void Update()
        {
            HandleBehaviour();
            FacePlayer();
            UpdateMovementAnimation();
        }
        
        protected abstract void HandleBehaviour();
        
        
        //Movement
        private void FacePlayer()
        {
            if (player.transform.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        
        protected bool IsPlayerInRange()
        {
            return Vector2.Distance(transform.position, player.transform.position) <= detectionRange;
        }

        protected void MoveTowardsPlayer()
        {
            Vector2 direction = player.transform.position - transform.position;
            rb.linearVelocity = direction * moveSpeed;
        }

        protected void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;
        }

        protected void UpdateMovementAnimation()
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.01f;
            if (isMoving)
            {
                animator.speed = 1f;
            }
            else
            {
                animator.speed = 0f;
            }
        }
        
        
        //Damage
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Player player = other.gameObject.GetComponent<Player>();
                player.PlayerTakeDamage();
                ShowAttackSprite();
            }
        }

        private void ShowAttackSprite()
        {
            StartCoroutine(ShowAttackSpriteCoroutine());
        }

        private IEnumerator ShowAttackSpriteCoroutine()
        {
            animator.enabled = false;
            
            spriteRenderer.sprite = attackSprite;
            yield return new WaitForSeconds(attackSpriteDuration);
            spriteRenderer.sprite = defaultSprite;
            
            animator.enabled = true;
        }

        public void TakeDamage(int damage)
        {
            if (!canTakeDamage)
            {
                return;
            }
            
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
                return;
            }

            StartCoroutine(DamageCooldownCoRoutine());
        }

        private IEnumerator DamageCooldownCoRoutine()
        {
            canTakeDamage = false;
            spriteRenderer.color = damageColor;
            
            yield return new WaitForSeconds(damageCooldown);
            
            spriteRenderer.color = originalColor;
            canTakeDamage = true;
        }
    }
}