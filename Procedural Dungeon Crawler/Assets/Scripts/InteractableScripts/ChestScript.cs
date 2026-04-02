using PlayerScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using DungeonLayoutGeneration.Settings;

namespace InteractableScripts
{
    public class ChestScript : MonoBehaviour
    {
        [SerializeField] private UIScripts.ChestRewardManager chestRewardManager;
        
        private DungeonSettings dungeonSettings;
        
        private Tilemap tilemap;
        private Vector3Int position;
        private Vector3 worldPosition;
        private bool chestOpened;

        public void SetRewardManager(UIScripts.ChestRewardManager rewardManager)
        {
            chestRewardManager = rewardManager;
        }
        
        public void InitialiseChest(Tilemap decorationsTilemap, Vector3Int pos, DungeonSettings settings)
        {
            tilemap = decorationsTilemap;
            position = pos;
            dungeonSettings = settings;
        }
        
        public void Interact(Player player)
        {
            if (chestOpened)
            {
                return;
            }
            
            chestOpened = true;
            tilemap.SetTile(position, dungeonSettings.OpenChestTile);
            chestRewardManager.OpenChestReward(player);

            Destroy(gameObject);
        }
    }
}