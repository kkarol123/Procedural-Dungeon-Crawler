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

        [SerializeField] int floorNumber = 1;
        public int FloorNumber => floorNumber;
        
        private void Start()
        {
            floorNumber = 1;
            RunData.FloorsReached = floorNumber;
            
            dungeonGenerator.SetSeed(Random.Range(int.MinValue, int.MaxValue));
            dungeonGenerator.GenerateDungeon();
            SpawnPlayerAtStartDoor();
        }


        private void SpawnPlayerAtStartDoor()
        {
            Vector3Int spawnDoor = dungeonGenerator.StartDoorPosition + Vector3Int.down;
            Vector3 spawnDoorWorldPosition = doorsTilemap.GetCellCenterWorld(spawnDoor);
            
            player.transform.position = new Vector3(spawnDoorWorldPosition.x, spawnDoorWorldPosition.y - 0.2f, -4f);
            Camera.main.transform.position = new Vector3(spawnDoorWorldPosition.x, spawnDoorWorldPosition.y, 0f);  //avoid having the camera follow the player immediately
        }

        public void LoadNextFloor()
        {
            if (dungeonGenerator == null)
            {
                return;
            }
            
            floorNumber++;
            RunData.FloorsReached = floorNumber;
            
            dungeonGenerator.SetSeed(Random.Range(int.MinValue, int.MaxValue));
            
            player.GiveKey();
            
            dungeonGenerator.GenerateDungeon();
            SpawnPlayerAtStartDoor();
        }
    }
}