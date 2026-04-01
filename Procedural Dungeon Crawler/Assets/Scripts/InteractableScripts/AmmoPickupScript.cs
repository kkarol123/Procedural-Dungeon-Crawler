using UnityEngine;
using PlayerScripts;

namespace InteractableScripts
{
    public class AmmoPickupScript : MonoBehaviour
    {
        public void Interact(Player player)
        {
            player.AddAmmoInReserve();
            Destroy(gameObject);
        }
    }
}