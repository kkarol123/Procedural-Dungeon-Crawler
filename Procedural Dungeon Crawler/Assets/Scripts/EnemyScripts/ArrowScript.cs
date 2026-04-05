using UnityEngine;
using PlayerScripts;

namespace EnemyScripts
{
    public class ArrowScript : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 4f;
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.gameObject.GetComponent<Player>();
                player.PlayerTakeDamage();
                Destroy(gameObject);
            }
            else if (other.CompareTag("Wall") || other.CompareTag("Door"))
            {
                Destroy(gameObject);
            }
        }
    }
}