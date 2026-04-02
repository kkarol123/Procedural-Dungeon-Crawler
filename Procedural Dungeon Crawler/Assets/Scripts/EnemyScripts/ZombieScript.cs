namespace EnemyScripts
{
    public class ZombieScript : EnemyScript
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