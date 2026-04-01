using PlayerScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InteractableScripts
{
    public class ChestScript : MonoBehaviour
    {
        [SerializeField] private UIScripts.ChestRewardManager chestRewardManager;
        
        private Tilemap tilemap;
        private Vector3Int position;
        private Vector3 worldPosition;
        
        public void InitialiseChest(Tilemap decorationsTilemap, Vector3Int pos)
        {
            tilemap = decorationsTilemap;
            position = pos;
        }
        
        public void Interact(Player player)
        {
            chestRewardManager.OpenChestReward(player);
        }
    }
}