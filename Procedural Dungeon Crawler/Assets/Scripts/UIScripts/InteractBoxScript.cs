using InteractableScripts;
using UnityEngine;
using PlayerScripts;

namespace UIScripts
{
    public class InteractBoxScript : MonoBehaviour
    {
        [SerializeField] private GameObject interactBox;
        private Player player;
        private bool playerInRange;
        
        private DoorScript doorScript;
        private AmmoPickupScript ammoScript;
        private MedkitPickupScript medkitScript;
        private SecretWallScript secretWallScript;
        private LadderScript ladderScript;
        private ChestScript chestScript;
        


        private void Start()
        {
            doorScript = GetComponentInParent<DoorScript>();
            ammoScript = GetComponentInParent<AmmoPickupScript>();
            medkitScript = GetComponentInParent<MedkitPickupScript>();
            secretWallScript = GetComponentInParent<SecretWallScript>();
            ladderScript = GetComponentInParent<LadderScript>();
            chestScript = GetComponentInParent<ChestScript>();
        }


        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                if (doorScript != null)
                {
                    doorScript.Interact(player);
                    return;
                }

                if (ammoScript != null)
                {
                    ammoScript.Interact(player);
                    return;
                }

                if (medkitScript != null)
                {
                    medkitScript.Interact(player);
                    return;
                }

                if (secretWallScript != null)
                {
                    secretWallScript.Interact();
                    return;
                }
                
                if (ladderScript != null)
                {
                    ladderScript.Interact(player);
                    return;
                }
                
                if (chestScript != null)
                {
                    chestScript.Interact(player);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                player = other.GetComponent<Player>();
                interactBox.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                player = null;
                interactBox.SetActive(false);
            }
        }
    }
}