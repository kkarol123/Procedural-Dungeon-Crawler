using UnityEngine;

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
}