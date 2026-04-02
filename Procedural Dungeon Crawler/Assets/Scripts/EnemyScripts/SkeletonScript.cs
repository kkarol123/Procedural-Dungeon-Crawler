namespace EnemyScripts
{
    public class SkeletonScript : EnemyScript
    {
        protected override void HandleBehaviour()
        {
            if (IsPlayerInRange())
            {
                MoveTowardsPlayer();
            }
            else
            {
                StopMoving();
            }
        }
    }
}