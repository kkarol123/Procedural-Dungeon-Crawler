using UnityEngine;
using PlayerScripts;

namespace CorridorTrapScripts.ArrowTrap
{
    public class CorridorArrow : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float lifetime = 3f;

        private Vector2 direction;

        public void Initialise(Vector2 shootDirection)
        {
            direction = shootDirection.normalized;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(Time.deltaTime * moveSpeed * direction);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.PlayerTakeDamage();
                Destroy(gameObject);
                return;
            }
            
            if (other.name == "Walls" || other.name == "PressurePlate(Clone)")
            {
                Destroy(gameObject);
            }
        }
    }
}