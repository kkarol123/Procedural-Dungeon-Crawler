using PlayerScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using UIScripts;

namespace InteractableScripts
{
    public class SignScript : MonoBehaviour
    {
        private SignUIManager signUIManager;

        public void SetSignUIManager(SignUIManager manager)
        {
            signUIManager = manager;
        }

        public void Interact(Player player)
        {
            signUIManager.OpenSign(player);
        }
    }
}