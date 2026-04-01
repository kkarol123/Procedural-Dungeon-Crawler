using PlayerScripts;
using UnityEngine;
using GameManagerScripts;

namespace InteractableScripts
{
    public class LadderScript : MonoBehaviour
    {
        private Vector3 teleportPosition;
        
        public void InitialiseLadder(Vector3 destination)
        {
            teleportPosition = destination;
        }
        
        public void Interact(Player player)
        {
            ScreenFadeManager.Instance.FadeTeleport(player, teleportPosition);
        }
    }
}