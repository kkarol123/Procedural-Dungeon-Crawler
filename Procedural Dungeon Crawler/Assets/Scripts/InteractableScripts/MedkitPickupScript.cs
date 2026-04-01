using PlayerScripts;
using UnityEngine;

namespace InteractableScripts
{
    public class MedkitPickupScript : MonoBehaviour
    {
        public void Interact(Player player)
        {
            player.PlayerGetHealth();
            Destroy(gameObject);
        }
    }
}