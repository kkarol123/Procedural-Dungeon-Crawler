using EnemyScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class BulletScript : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 12f;
        [SerializeField] private float lifeTime = 2f;
    
        private Rigidbody2D rb;
        private int damage;

        public void Initialise(int bulletDamage)
        {
            damage = bulletDamage;
        }
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = -transform.up * bulletSpeed;
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Wall") || other.CompareTag("Door"))
            {
                Destroy(gameObject);
            }
            else if (other.CompareTag("Enemy"))
            {
                EnemyScript enemy = other.GetComponent<EnemyScript>();
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}