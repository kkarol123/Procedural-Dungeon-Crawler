using UnityEngine;
using PlayerScripts;
using System.Collections;

namespace CorridorTrapScripts.ArrowTrap
{
    public class PressurePlateTrap : MonoBehaviour
    {
        [SerializeField] private float triggerDelay = 0.2f;

        private Vector3Int launcherPosition;
        private Vector2Int fireDirection;
        private bool triggered;

        public void InitialiseTrap(Vector3Int launcherPos, Vector2Int direction)
        {
            launcherPosition = launcherPos;
            fireDirection = direction;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered)
            {
                return;
            }
            
            Player player = other.GetComponent<Player>();
			if (player == null){
				return;
			}

            StartCoroutine(TriggerTrapCoroutine());
        }

        private IEnumerator TriggerTrapCoroutine()
        {
            triggered = true;
            
            yield return new WaitForSeconds(triggerDelay);

            ArrowTrapManager.Instance.FireArrow(launcherPosition, fireDirection);
        }
    }
}