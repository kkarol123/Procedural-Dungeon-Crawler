using UnityEngine;
using PlayerScripts;

namespace InteractableScripts
{
    public class KeyScript : MonoBehaviour
    {
        public void Interact(Player player)
        {
            player.GiveKey();
            Destroy(gameObject);
        }
    }
}