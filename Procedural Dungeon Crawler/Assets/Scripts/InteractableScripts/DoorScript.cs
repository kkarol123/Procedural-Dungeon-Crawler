using UnityEngine;
using UnityEngine.Tilemaps;
using PlayerScripts;

namespace InteractableScripts
{
    public class DoorScript : MonoBehaviour
    {
        private Tilemap doorsTilemap;
        private Vector3Int doorPosition;

        public void InitialiseDoor(Tilemap tilemap, Vector3Int position)
        {
            doorsTilemap = tilemap;
            doorPosition = position;
        }
        
        public void Interact(Player player)
        {
            if (player.HasKey)
            {
                player.UseKey();
                doorsTilemap.SetTile(doorPosition, null);
                Destroy(gameObject);
            }
        }
    }
}