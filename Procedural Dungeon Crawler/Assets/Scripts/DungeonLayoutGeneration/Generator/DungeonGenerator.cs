using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;
using DungeonLayoutGeneration.Settings;
using System.Collections.Generic;

namespace DungeonLayoutGeneration.Generator
{
    public enum RoomType
    {
        Rectangle,
        Circle,
        Cross
    }

    [System.Serializable]
    public class RoomData
    {
        public Vector3Int center;
        public int width;
        public int height;
        public RoomType type;

        public RoomData(Vector3Int center, int width, int height, RoomType type)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.type = type;
        }
    }
    
    [ExecuteAlways]
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private DungeonSettings dungeonSettings;
        [SerializeField] private Tilemap terrainTilemap;
        
        private int numberOfRooms;
        private int distance;
        
        private List<RoomData> rooms = new List<RoomData>();
        
        [Button("Generate Platformer")]
        public void GeneratePlatformer()
        {
            //0. Reset the previous tilemaps and List
            terrainTilemap.ClearAllTiles();
            rooms.Clear(); 
            
            //1. Initialize Random/Seed
            System.Random rng = new System.Random(dungeonSettings.Seed);
            
            //2. Generate Rooms
            GenerateRooms(rng);
            
            //3. Add Corridors
            GenerateCorridorFloors();
            GenerateCorridorWalls();
        }
        

        
        
        
        //Room functions
        private void GenerateRooms(System.Random rng)
        {
            Vector3Int currentPos = new Vector3Int(0, 0, 0);
            
            numberOfRooms = rng.Next(dungeonSettings.MinRooms, dungeonSettings.MaxRooms + 1);
            distance = rng.Next(dungeonSettings.MinDistance, dungeonSettings.MaxDistance + 1);
            
            for (int i = 0; i < numberOfRooms; i++)
            {
                RoomType roomType = (RoomType)rng.Next(0, 3); //0 is a rectangle, 1 is a circle, 2 is a UShaped room

                int width = rng.Next(dungeonSettings.MinWidth, dungeonSettings.MaxWidth + 1);
                int height = rng.Next(dungeonSettings.MinHeight, dungeonSettings.MaxHeight + 1);


                if (IsOverlapping(currentPos, width, height) == false)
                {
                    switch (roomType)
                    {
                        case RoomType.Rectangle:
                            DrawRectangleRoom(currentPos, width, height);
                            break;
                        case RoomType.Circle:
                            DrawCircleRoom(currentPos, width, height);
                            break;
                        case RoomType.Cross:
                            DrawCrossRoom(currentPos, width, height);
                            break;
                    }    
                    
                    rooms.Add(new RoomData(currentPos, width, height, roomType));
                }
                else
                {
                    i--;  //try drawing room again
                }
                
                
                int direction = rng.Next(0, 3);
                switch (direction)
                {
                    case 0: //right
                        currentPos += new Vector3Int(width + distance, 0, 0);
                        break;
                    case 1: //up
                        currentPos += new Vector3Int(0, height + distance, 0);
                        break;
                    case 2:
                        currentPos += new Vector3Int(0, -(height + distance), 0);
                        break;
                }
            }
        }

        private bool IsOverlapping(Vector3Int center, int width, int height)
        {
            int startX = center.x - width / 2;
            int startY = center.y - height / 2;

            for (int x = -1; x <= width; x++)
            {
                for (int y = -1; y <= height; y++)
                {
                    Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);

                    if (terrainTilemap.HasTile(tilePos))
                    {
                        return true;
                    }
                }
            }

            return false;
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
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                    }
                    else
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.FloorTile);
                    }
                }
            }
        }

        private void DrawCircleRoom(Vector3Int center, int width, int height)
        {
            float radiusX = width / 2f;
            float radiusY = height / 2f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int tileX = center.x - width / 2 + x;
                    int tileY = center.y - height / 2 + y;

                    Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);

                    float normalizedX = (x - radiusX + 0.5f) / radiusX;
                    float normalizedY = (y - radiusY + 0.5f) / radiusY;

                    float distanceFromCenter = normalizedX * normalizedX + normalizedY * normalizedY;
                    
                    
                    if (distanceFromCenter <= 1f)
                    {
                        float wallThreshold = 0.625f;

                        if (distanceFromCenter >= wallThreshold)
                        {
                            terrainTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                        }
                        else
                        {
                            terrainTilemap.SetTile(tilePos, dungeonSettings.FloorTile);
                        }
                    }
                }
            }
        }

        private void DrawCrossRoom(Vector3Int center, int width, int height)
        {
            int startX = center.x - width / 2;
            int startY = center.y - height / 2;

            int armWidth = Mathf.Max(3, width / 3);
            int armHeight = Mathf.Max(3, height / 3);

            int verticalStartX = (width - armWidth) / 2;
            int verticalEndX = verticalStartX + armWidth - 1;

            int horizontalStartY = (height - armHeight) / 2;
            int horizontalEndY = horizontalStartY + armHeight - 1;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool isInVerticalArm = x >= verticalStartX && x <= verticalEndX;
                    bool isInHorizontalArm = y >= horizontalStartY && y <= horizontalEndY;
                    if (!(isInVerticalArm || isInHorizontalArm))
                    {
                        continue;
                    }
                    
                    Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);
                    bool isWall = false;

                    //Check the 4 neighbors to see if this tile is on the outer edge of the cross
                    bool leftFilled = (x - 1 >= 0) && ((x - 1 >= verticalStartX && x - 1 <= verticalEndX) || (y >= horizontalStartY && y <= horizontalEndY));
                    bool rightFilled = (x + 1 < width) && ((x + 1 >= verticalStartX && x + 1 <= verticalEndX) || (y >= horizontalStartY && y <= horizontalEndY));
                    bool downFilled = (y - 1 >= 0) && ((x >= verticalStartX && x <= verticalEndX) || (y - 1 >= horizontalStartY && y - 1 <= horizontalEndY));
                    bool upFilled = (y + 1 < height) && ((x >= verticalStartX && x <= verticalEndX) || (y + 1 >= horizontalStartY && y + 1 <= horizontalEndY));
                    if (!leftFilled || !rightFilled || !downFilled || !upFilled)
                    {
                        isWall = true;
                    }

                    if (isWall)
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                    }
                    else
                    {
                        terrainTilemap.SetTile(tilePos, dungeonSettings.FloorTile);
                    }
                }
            }
        }
        
        
        

        //Corridor functions
        private void GenerateCorridorFloors()
        {
            for (int i = 0; i < rooms.Count - 1; i++) 
            { 
                Vector3Int start = rooms[i].center; 
                Vector3Int end = rooms[i + 1].center; 
                
                DrawHorizontalCorridor(start.y, start.x, end.x);
                DrawVerticalCorridor(end.x, start.y, end.y);
            } 
        }

        private void DrawHorizontalCorridor(int y, int startX, int endX)
        {
            for (int x = Mathf.Min(startX, endX); x <= Mathf.Max(startX, endX); x++)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    Vector3Int floorPos = new Vector3Int(x, y + offset, 0); 
                    terrainTilemap.SetTile(floorPos, dungeonSettings.FloorTile);
                } 
            }
        }

        private void DrawVerticalCorridor(int x, int startY, int endY)
        {
            for (int y = Mathf.Min(startY, endY); y <= Mathf.Max(startY, endY); y++)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    Vector3Int floorPos = new Vector3Int(x + offset, y, 0);
                    terrainTilemap.SetTile(floorPos, dungeonSettings.FloorTile);
                } 
            }
        }
        
        private void GenerateCorridorWalls()
        {
            BoundsInt bounds = terrainTilemap.cellBounds;

            for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
            {
                for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    // Only consider empty tiles
                    if (terrainTilemap.HasTile(pos))
                        continue;

                    bool hasFloorNeighbour =
                        terrainTilemap.GetTile(new Vector3Int(x + 1, y, 0)) == dungeonSettings.FloorTile ||
                        terrainTilemap.GetTile(new Vector3Int(x - 1, y, 0)) == dungeonSettings.FloorTile ||
                        terrainTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == dungeonSettings.FloorTile ||
                        terrainTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == dungeonSettings.FloorTile;

                    if (hasFloorNeighbour)
                    {
                        terrainTilemap.SetTile(pos, dungeonSettings.WallTile);
                    }
                }
            }
        }
        
        
        
        
        //Reset button functions
        [Button("Reset Tilemaps")]
        public void ResetTerrain()
        {
            terrainTilemap.ClearAllTiles();
        }
    }
}
