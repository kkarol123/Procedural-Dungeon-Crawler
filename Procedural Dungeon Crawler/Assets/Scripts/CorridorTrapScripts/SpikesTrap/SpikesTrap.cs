using UnityEngine;
using System.Collections;
using PlayerScripts;

namespace TrapScripts
{
    public class SpikesTrap : MonoBehaviour
    {
        [SerializeField] private Sprite notTriggeredSprite;
        [SerializeField] private Sprite triggeredSprite;

        private float triggerDelay = 0.5f;
        private float activeTime = 0.6f;
        private float resetDelay = 0.2f;

        private SpriteRenderer spriteRenderer;

        private bool playerOnTrap;
        private bool isTriggered;
        private Player player;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = notTriggeredSprite;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            player = other.GetComponent<Player>();
            playerOnTrap = true;

            if (isTriggered)
            {
                player.PlayerTakeDamage();
            }
            else
            {
                StartCoroutine(TriggerTrapCoroutine());
            }
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            playerOnTrap = false;
        }

        private IEnumerator TriggerTrapCoroutine()
        {
            isTriggered = true;
            yield return new WaitForSeconds(triggerDelay);
            spriteRenderer.sprite = triggeredSprite;

            if (playerOnTrap && player != null)
            {
                player.PlayerTakeDamage();
            }
            
            yield return new WaitForSeconds(activeTime);
            
            yield return new WaitForSeconds(resetDelay);
            
            spriteRenderer.sprite = notTriggeredSprite;
            isTriggered = false;

            //if player for some reason never left the trap, retrigger again
            if (playerOnTrap)
            {
                StartCoroutine(TriggerTrapCoroutine());
            }
        }
    }
}