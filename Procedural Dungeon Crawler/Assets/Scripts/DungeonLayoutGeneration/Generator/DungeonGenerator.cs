using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;
using DungeonLayoutGeneration.Settings;
using UnityEngine.Rendering;

namespace DungeonLayoutGeneration.Generator
{
    public enum RoomType
    {
        Rectangle,
        Circle,
        UShape
    }
    
    [ExecuteAlways]
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private DungeonSettings dungeonSettings;
        [SerializeField] private Tilemap terrainTilemap;
        
        private int numberOfRooms;
        private int distance;
        
        [Button("Generate Platformer")]
        public void GeneratePlatformer()
        {
            //0. Reset the previous tilemaps
            terrainTilemap.ClearAllTiles();
            
            //1. Initialize Random/Seed
            System.Random rng = new System.Random(dungeonSettings.Seed);
            
            //2. Generate Rooms
            GenerateRooms(rng);
        }
        

        private void GenerateRooms(System.Random rng)
        {
            Vector3Int currentPos = new Vector3Int(0, 0, 0);
            
            numberOfRooms = rng.Next(dungeonSettings.MinRooms, dungeonSettings.MaxRooms + 1);
            distance = rng.Next(dungeonSettings.MinDistance, dungeonSettings.MaxDistance + 1);
            
            for (int i = 0; i < numberOfRooms; i++)
            {
                RoomType roomType = (RoomType)rng.Next(0, 1); //0 is a rectangle, 1 is a circle, 2 is a UShaped room

                int width = rng.Next(dungeonSettings.MinWidth, dungeonSettings.MaxWidth + 1);
                int height = rng.Next(dungeonSettings.MinHeight, dungeonSettings.MaxHeight + 1);
                
                switch (roomType)
                {
                    case RoomType.Rectangle:
                        DrawRectangleRoom(currentPos, width, height);
                        break;
                    case RoomType.Circle:
                        DrawCircleRoom();
                        break;
                    case RoomType.UShape:
                        DrawUShapeRoom();
                        break;
                }
                
                currentPos += new Vector3Int(width + distance, 0, 0);
            }
        }

        private void DrawRectangleRoom(Vector3Int center, int width, int height)
        {
            int startX = center.x - width / 2;
            int startY = center.y - height / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);
                    
                    //if the tile position is on the perimeter, draw a wall tile, else draw the floor tile
                    if (x == 0 || x == width - 1)
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.SideWallTile);
                    }
                    else if (y == 0)
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.TopWallTile);
                    }
                    else if (y == height - 1)
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.BottomWallTile);
                    }
                    else
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.FloorTile);
                    }
                }
            }
        }

        private void DrawCircleRoom()
        {
            
        }

        private void DrawUShapeRoom()
        {
            
        }
        
        [Button("Reset Tilemaps")]
        public void ResetTerrain()
        {
            terrainTilemap.ClearAllTiles();
        }
    }
}
