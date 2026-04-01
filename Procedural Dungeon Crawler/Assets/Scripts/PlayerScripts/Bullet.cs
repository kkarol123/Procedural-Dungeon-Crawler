using UnityEngine;

namespace PlayerScripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 12f;
        [SerializeField] private float lifeTime = 2f;
    
        private Rigidbody2D rb;
        private Vector2 moveDirection;
    
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
                //implement
            }
        }
    }
}