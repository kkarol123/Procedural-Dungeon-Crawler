using UnityEngine;

namespace EnemyScripts
{
    public class SpiderScript : EnemyScript
    {
        [SerializeField] private float patrolDistance = 2.5f;
        [SerializeField] private float patrolWaitTime = 0.5f;
        [SerializeField] private float stuckTimeThreshold = 0.35f;
        [SerializeField] private float minimumProgressDistance = 0.01f;
        private Vector3 lastPositon;

        private Vector3 patrolCenter;
        private Vector3 leftPoint;
        private Vector3 rightPoint;
        private Vector3 currentTarget;

        private bool movingToRight = true;
        private float waitTimer;
        private float stuckTimer;

        protected override void Start()
        {
            base.Start();

            patrolCenter = transform.position;
            leftPoint = patrolCenter + Vector3.left * patrolDistance;
            rightPoint = patrolCenter + Vector3.right * patrolDistance;

            currentTarget = rightPoint;
            lastPositon = transform.position;
        }
        
        protected override void HandleBehaviour()
        {
            
            if (IsPlayerInRange())
            {
                waitTimer = 0f;
                stuckTimer = 0f;
                lastPositon = transform.position;
                MoveTowardsPlayer();
                return;
            }

            Patrol();
        }

        private void Patrol()
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget);

            if (distanceToTarget <= 0.1f)
            {
                StopMoving();
                stuckTimer = 0f;

                waitTimer += Time.deltaTime;

                if (waitTimer >= patrolWaitTime)
                {
                    waitTimer = 0f;
                    SwitchPatrolDirection();
                }
                
                return;
            }

            waitTimer = 0f;
            
            Vector2 direction = ((Vector2)currentTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }

            CheckIfStuck();
        }

        private void CheckIfStuck()
        {
            float movedDistance = Vector2.Distance(transform.position, lastPositon);
            
            if (movedDistance <= minimumProgressDistance)
            {
                stuckTimer += Time.deltaTime;

                if (stuckTimer >= stuckTimeThreshold)
                {
                    stuckTimer = 0f;
                    StopMoving();
                    SwitchPatrolDirection();
                }
            }
            else
            {
                stuckTimer = 0f;
            }
            
            lastPositon = transform.position;
        }

        private void SwitchPatrolDirection()
        {
            movingToRight = !movingToRight;

            if (movingToRight)
            {
                currentTarget = rightPoint;
            }
            else
            {
                currentTarget = leftPoint;
            }
        }
        
        
        


        protected override void FacePlayer()
        {
            if (IsPlayerInRange())
            {
                base.FacePlayer();
            }
        }
    }
}