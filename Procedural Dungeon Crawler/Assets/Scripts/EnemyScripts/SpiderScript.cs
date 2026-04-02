namespace EnemyScripts
{
    public class SpiderScript : EnemyScript
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