using UnityEngine;
using PlayerScripts;

namespace  UIScripts
{
    public class InteractBoxScript : MonoBehaviour
    {
        [SerializeField] private GameObject interactBox;

        private bool playerInRange;

        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interacted");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                interactBox.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                interactBox.SetActive(false);
            }
        }
    }
}