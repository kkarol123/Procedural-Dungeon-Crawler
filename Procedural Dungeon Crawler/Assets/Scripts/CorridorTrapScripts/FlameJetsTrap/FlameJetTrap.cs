using UnityEngine;
using System.Collections;
using PlayerScripts;

namespace CorridorTrapScripts.FlameJetsTrap
{
    public class FlameJetTrap : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer flameSpriteRenderer;
        [SerializeField] private Collider2D flameTriggerCollider;

        [SerializeField] private float activeTime = 2f;
        [SerializeField] private float inactiveTime = 2f;

        private bool flameIsActive;

        private void Start()
        {
            SetFlameState(false);
            StartCoroutine(FlameCycleCoroutine());
        }

        private IEnumerator FlameCycleCoroutine()
        {
            while (true)
            {
                SetFlameState(true);
                yield return new WaitForSeconds(activeTime);

                SetFlameState(false);
                yield return new WaitForSeconds(inactiveTime);
            }
        }

        private void SetFlameState(bool active)
        {
            flameIsActive = active;
            flameSpriteRenderer.enabled = active;
            flameTriggerCollider.enabled = active;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!flameIsActive)
            {
                return;
            }
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                return;
            }
            
            player.PlayerTakeDamage();
        }
    }
}