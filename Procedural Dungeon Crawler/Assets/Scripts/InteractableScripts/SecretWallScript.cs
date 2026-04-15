using UnityEngine;
using UnityEngine.Tilemaps;

namespace InteractableScripts
{
    public class SecretWallScript : MonoBehaviour
    {
        private Tilemap wallsTilemap;
        private Tilemap ladderTilemap;

        private Vector3Int wallPosition;

        private TileBase ladderTile;
        private GameObject ladderPrefab;
        private Vector3 ladderTeleportDestination;

        public void InitialiseSecretWall(Tilemap wallsMap, Tilemap ladderMap, Vector3Int pos, TileBase ladderTileToPlace, GameObject ladderObjectPrefab, Vector3 teleportDestination)
        {
            wallsTilemap = wallsMap;
            ladderTilemap = ladderMap;
            wallPosition = pos;
            ladderTile = ladderTileToPlace;
            ladderPrefab = ladderObjectPrefab;
            ladderTeleportDestination = teleportDestination;
        }

        public void Interact()
        {
            ladderTilemap.SetTile(wallPosition, ladderTile);
            
            Vector3 worldPosition = wallsTilemap.GetCellCenterWorld(wallPosition);
            GameObject ladderObject = Instantiate(ladderPrefab, worldPosition, Quaternion.identity, transform.parent);
            
            LadderScript ladderScript = ladderObject.GetComponent<LadderScript>();
            ladderScript.InitialiseLadder(ladderTeleportDestination);
            
            Destroy(gameObject);
        }
    }
}