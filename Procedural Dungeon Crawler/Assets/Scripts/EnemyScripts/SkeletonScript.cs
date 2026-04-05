using UnityEngine;

namespace EnemyScripts
{
    public class SkeletonScript : EnemyScript
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private Transform bowTip;
        [SerializeField] private float shootCooldown = 2f;
        [SerializeField] private float arrowSpeed = 6f;

        private float shootTimer;
        
        
        protected override void HandleBehaviour()
        {
            StopMoving();
            
            if (!IsPlayerInRange())
            {
                return;
            }

            shootTimer += Time.deltaTime;

            if (shootTimer >= shootCooldown)
            {
                shootTimer = 0f;
                ShootArrow();
            }
        }

        private void ShootArrow()
        {
            Vector2 direction = (player.transform.position - bowTip.position).normalized;
            
            GameObject arrowObject = Instantiate(arrowPrefab, bowTip.position, Quaternion.identity);
            
            Rigidbody2D arrowRb = arrowObject.GetComponent<Rigidbody2D>();
            arrowRb.linearVelocity = direction * arrowSpeed;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}