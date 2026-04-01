using UnityEngine;
using DungeonLayoutGeneration.Generator;
using PlayerScripts;
using UnityEngine.Tilemaps;

namespace GameManagerScripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DungeonGenerator dungeonGenerator;
        [SerializeField] private Player player;
        [SerializeField] private Tilemap doorsTilemap;
        
        private void Start()
        {
            dungeonGenerator.GenerateDungeon();
            SpawnPlayerAtStartDoor();
        }


        private void SpawnPlayerAtStartDoor()
        {
            Vector3Int spawnDoor = dungeonGenerator.StartDoorPosition + Vector3Int.down;
            Vector3 spawnDoorWorldPosition = doorsTilemap.GetCellCenterWorld(spawnDoor);
            
            player.transform.position = new Vector3(spawnDoorWorldPosition.x, spawnDoorWorldPosition.y, -4f);
            Camera.main.transform.position = new Vector3(spawnDoorWorldPosition.x, spawnDoorWorldPosition.y, 0f);  //avoid having the camera follow the player immediately
        }
    }
}