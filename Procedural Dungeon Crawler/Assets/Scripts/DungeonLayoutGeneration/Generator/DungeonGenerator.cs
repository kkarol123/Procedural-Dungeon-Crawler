using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;
using DungeonLayoutGeneration.Settings;
using System.Collections.Generic;
using InteractableScripts;

namespace DungeonLayoutGeneration.Generator
{
    public enum RoomType
    {
        Rectangle,
        Circle,
        Cross
    }

    public enum BiomeType
    {
        Ruined,
        Flooded,
        Volcanic
    }

    [System.Serializable]
    public class RoomData
    {
        public Vector3Int center;
        public int width;
        public int height;
        public BiomeType biome;

        public RoomData(Vector3Int center, int width, int height, BiomeType biome)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.biome = biome;
        }
    }
    
    

    [System.Serializable]
    public class EnemySpawnData
    {
        public Vector3Int position;
        public int roomIndex;
        public EnemyType enemyType;

        public EnemySpawnData(Vector3Int pos, int room, EnemyType type)
        {
            position = pos;
            roomIndex = room;
            enemyType = type;
        }
    }

    public enum EnemyType
    {
        Zombie,
        Skeleton,
        Spider
    }
    
    
    
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private DungeonSettings dungeonSettings;
        
        [SerializeField] private Tilemap floorsTilemap;
        [SerializeField] private Tilemap wallsTilemap;
        [SerializeField] private Tilemap doorsTilemap;
        [SerializeField] private Tilemap collisionDecorationTilemap; 
        [SerializeField] private Tilemap nonCollisionDecorationTilemap;
        [SerializeField] private Tilemap backgroundTilemap;
        
        private int numberOfRooms;
        private int distance;
        
        private List<RoomData> rooms = new List<RoomData>();

        private Vector3Int secretRoomCenter;
        private Vector3Int secretRoomLadderPosition;
        private GameObject secretRoomLadderObject;
        private Vector3Int dungeonLadderPosition;
        
        private Vector3Int startDoorPosition;
        public Vector3Int StartDoorPosition => startDoorPosition;
        
        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private GameObject ammoPickupPrefab;
        [SerializeField] private GameObject medkitPickupPrefab;
        [SerializeField] private GameObject chestPrefab;
        [SerializeField] private GameObject secretWallPrefab;
        [SerializeField] private GameObject ladderPrefab;
        [SerializeField] private GameObject signPrefab;

        private List<EnemySpawnData> enemySpawnPoints = new List<EnemySpawnData>();
        public List<EnemySpawnData> EnemySpawnPoints => enemySpawnPoints;
        [SerializeField] private GameObject zombieEnemyPrefab;
        [SerializeField] private GameObject skeletonEnemyPrefab;
        [SerializeField] private GameObject spiderEnemyPrefab;
            
            
        
        [Button("Generate Platformer")]
        public void GenerateDungeon()
        {
            //0. Reset the previous tilemaps and List
            ResetGrid();
            
            
            //1. Initialize Random/Seed 
            System.Random rng = new System.Random(dungeonSettings.Seed);
            
            //2. Generate Rooms              
            GenerateRooms(rng);
            
            //3. Generate Start and End Room  
            GenerateStartRoom();
            GenerateEndRoom();
            
            //4. Add Corridors
            GenerateCorridorFloors();
            GenerateCorridorWalls();
            
            //5. Add Floor Variation
            ApplyFloorVariation();
            
            //6. Add Secret Room
            GenerateSecretRoom();
            GenerateSecretWall(rng);
            GenerateSecretSign(rng, dungeonLadderPosition);
            
            //6. Add decorations
            ApplyDecorations(rng);
            
            //7. Spawn Pickups
            GeneratePickups(rng);
            
            
            //8. Generate enemy spawn points
            GenerateEnemySpawnPoints(rng);
            
            
            //9. Add Background
            ApplyBackgroundTiles();
        }
        

        
        
        
        //Room functions
        private void GenerateRooms(System.Random rng)
        {
            Vector3Int currentPos = new Vector3Int(0, 0, 0);
            
            numberOfRooms = rng.Next(dungeonSettings.MinRooms, dungeonSettings.MaxRooms + 1);
            distance = rng.Next(dungeonSettings.MinDistance, dungeonSettings.MaxDistance + 1);

            int roomsCreated = 0;
            int attempts = 0;
            int maxAttempts = numberOfRooms * 20;

            while (roomsCreated < numberOfRooms && attempts < maxAttempts)
            {
                attempts++;
            
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

                    BiomeType biome = GetBiomeForRoomIndex(roomsCreated, numberOfRooms);

                    rooms.Add(new RoomData(currentPos, width, height, biome));
                    roomsCreated++;

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

                    if (floorsTilemap.HasTile(tilePos) || wallsTilemap.HasTile(tilePos))
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
                        wallsTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                    }
                    else
                    {
                        floorsTilemap.SetTile(tilePos, dungeonSettings.DryStoneTile);
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
                        float wallThreshold = 0.65f;

                        if (distanceFromCenter >= wallThreshold)
                        {
                            wallsTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                        }
                        else
                        {
                            floorsTilemap.SetTile(tilePos, dungeonSettings.DryStoneTile);
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
                        wallsTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                    }
                    else
                    {
                        floorsTilemap.SetTile(tilePos, dungeonSettings.DryStoneTile);
                    }
                }
            }
        }

        
        
        private void GenerateStartRoom()
        {
            RoomData firstRoom = rooms[0];

            int startRoomWidth = 9;
            int startRoomHeight = 7;
            
            Vector3Int startRoomCenter = new Vector3Int(firstRoom.center.x - firstRoom.width / 2 - startRoomWidth / 2 - 10, firstRoom.center.y, 0);
            
            DrawRectangleRoom(startRoomCenter, startRoomWidth, startRoomHeight);

            Vector3Int doorPos = new Vector3Int(startRoomCenter.x, startRoomCenter.y + startRoomHeight / 2, 0);
            startDoorPosition = doorPos;
            doorsTilemap.SetTile(doorPos, dungeonSettings.ExitDoorTile);

            rooms.Insert(0, new RoomData(startRoomCenter, startRoomWidth, startRoomHeight, BiomeType.Ruined));
        }

        private void GenerateEndRoom()
        {
            RoomData lastRoom =  rooms[rooms.Count - 1];
            
            int endRoomWidth = 9;
            int endRoomHeight = 7;
            
            Vector3Int endRoomCenter = new Vector3Int(lastRoom.center.x + lastRoom.width / 2 + endRoomWidth / 2 + 10, lastRoom.center.y, 0);
            
            DrawRectangleRoom(endRoomCenter, endRoomWidth, endRoomHeight);
            
            Vector3Int doorPos = new Vector3Int(endRoomCenter.x, endRoomCenter.y + endRoomHeight / 2, 0);
            doorsTilemap.SetTile(doorPos, dungeonSettings.ExitDoorTile);
            
            rooms.Insert(rooms.Count, new RoomData(endRoomCenter, endRoomWidth, endRoomHeight, BiomeType.Volcanic));
        }
        
        
        
        
        
        

        //Corridor functions
        private void GenerateCorridorFloors()
        {
            for (int i = 0; i < rooms.Count - 1; i++) 
            { 
                Vector3Int start = rooms[i].center; 
                Vector3Int end = rooms[i + 1].center; 
                
                GenerateHorizontalCorridor(start.y, start.x, end.x);
                GenerateVerticalCorridor(end.x, start.y, end.y);
                
                PlaceLockedDoor(rooms[i], rooms[i + 1]);
            } 
        }

        
        private void GenerateHorizontalCorridor(int y, int startX, int endX)
        {
            for (int x = Mathf.Min(startX, endX); x <= Mathf.Max(startX, endX); x++)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    Vector3Int floorPos = new Vector3Int(x, y + offset, 0); 
                    wallsTilemap.SetTile(floorPos, null);
                    floorsTilemap.SetTile(floorPos, dungeonSettings.DryStoneTile);
                } 
            }
        }

        private void GenerateVerticalCorridor(int x, int startY, int endY)
        {
            for (int y = Mathf.Min(startY, endY); y <= Mathf.Max(startY, endY); y++)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    Vector3Int floorPos = new Vector3Int(x + offset, y, 0);
                    wallsTilemap.SetTile(floorPos, null);
                    floorsTilemap.SetTile(floorPos, dungeonSettings.DryStoneTile);
                } 
            }
        }
        
        
        private void GenerateCorridorWalls()
        {
            BoundsInt bounds = wallsTilemap.cellBounds;

            for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
            {
                for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    //only consider empty tiles
                    if (wallsTilemap.HasTile(pos) || floorsTilemap.HasTile(pos))
                    {
                        continue;
                    }

                    bool hasFloorNeighbour =
                        floorsTilemap.GetTile(new Vector3Int(x + 1, y, 0)) == dungeonSettings.DryStoneTile ||
                        floorsTilemap.GetTile(new Vector3Int(x - 1, y, 0)) == dungeonSettings.DryStoneTile ||
                        floorsTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == dungeonSettings.DryStoneTile ||
                        floorsTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == dungeonSettings.DryStoneTile;

                    if (hasFloorNeighbour)
                    {
                        wallsTilemap.SetTile(pos, dungeonSettings.WallTile);
                    }
                }
            }
        }
        

        private void PlaceLockedDoor(RoomData startRoom, RoomData endRoom)
        {
            int changeX = endRoom.center.x - startRoom.center.x;
            int changeY = endRoom.center.y - startRoom.center.y;

            if (changeX != 0)
            {
                int directionX;
                if (changeX > 0)
                {
                    directionX = 1;
                }
                else
                {
                    directionX = -1;
                }
                
                int doorX = (startRoom.center.x + directionX * (startRoom.width / 2));
                int doorY = startRoom.center.y;
                
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY - 2, 0), dungeonSettings.WallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY - 1, 0), dungeonSettings.WallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), null);
                doorsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), dungeonSettings.DoorTile);
                SpawnDoorObject(new Vector3Int(doorX, doorY, 0));
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY + 1, 0), dungeonSettings.WallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY + 2, 0), dungeonSettings.WallTile);
            }
            else
            {
                int directionY;

                if (changeY > 0)
                {
                    directionY = 1;
                }
                else
                {
                    directionY = -1;
                }

                int doorX = startRoom.center.x;
                int doorY = startRoom.center.y + directionY * (startRoom.height / 2);
                
                wallsTilemap.SetTile(new Vector3Int(doorX - 2, doorY, 0), dungeonSettings.WallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX - 1, doorY, 0), dungeonSettings.WallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), null);
                doorsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), dungeonSettings.DoorTile);
                SpawnDoorObject(new Vector3Int(doorX, doorY, 0));
                wallsTilemap.SetTile(new Vector3Int(doorX + 1, doorY, 0), dungeonSettings.WallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX + 2, doorY, 0), dungeonSettings.WallTile);
            }
        }

        private void SpawnDoorObject(Vector3Int doorPosition)
        {
            Vector3 worldPosition = doorsTilemap.GetCellCenterWorld(doorPosition);
            GameObject doorObject = Instantiate(doorPrefab, worldPosition, Quaternion.identity, transform);
            
            DoorScript doorScript = doorObject.GetComponent<DoorScript>();
            doorScript.InitialiseDoor(doorsTilemap, doorPosition);
        }
        
        
        
        
        
        //Environmental Variation Functions
        private void ApplyFloorVariation()
        {
            BoundsInt bounds = wallsTilemap.cellBounds;

            float offsetX = dungeonSettings.Seed * 0.67f;
            float offsetY = dungeonSettings.Seed * 0.69f;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase currentTile = floorsTilemap.GetTile(pos);

                    if (currentTile == dungeonSettings.DryStoneTile)
                    {
                        RoomData room = GetRoomAtPosition(pos);

                        if (room == null) //if room is a corridor
                        {
                            room = GetNearestRoomToPosition(pos);
                        }
                        if (room != null) //if a room is found
                        {
                            TileBase variedTile = GetFloorVariationTile(pos, room.biome, offsetX, offsetY);

                            if (variedTile == dungeonSettings.LavaTile && IsNearDoor(pos))
                            {
                                variedTile = dungeonSettings.CrackedStoneTile;
                            }
                            
                            floorsTilemap.SetTile(pos, variedTile);
                        }
                    }
                }
            }
        }
        
        private TileBase GetFloorVariationTile(Vector3Int pos, BiomeType biome, float offsetX, float offsetY)
        {
            float sampleX = pos.x * dungeonSettings.NoiseScale + offsetX;
            float sampleY = pos.y * dungeonSettings.NoiseScale + offsetY;

            float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

            if (biome == BiomeType.Ruined)
            {
                if (noiseValue < 0.4f)
                {
                    return dungeonSettings.DryStoneTile;
                }
                if (noiseValue < 0.65f)
                {
                    return dungeonSettings.CrackedStoneTile;
                }
                
                return dungeonSettings.MossyStoneTile;
            }

            if (biome == BiomeType.Flooded)
            {
                if (noiseValue < 0.25f)
                {
                    return dungeonSettings.DryStoneTile;
                }
                if (noiseValue < 0.45f)
                {
                    return dungeonSettings.MossyStoneTile;
                }
                if (noiseValue < 0.625f)
                {
                    return dungeonSettings.DampStoneTile;
                }
                
                return dungeonSettings.PuddleTile;
            }
            
            //Volcanic
            if (noiseValue < 0.5f)
            {
                return dungeonSettings.CrackedStoneTile;
            }
            if (noiseValue < 0.70f)
            {
                return dungeonSettings.VolanicRubbleTile;
            }

            return dungeonSettings.LavaTile;
        }

        //Biomes
        private BiomeType GetBiomeForRoomIndex(int roomIndex, int totalRooms)
        {
            float progress = (float) roomIndex / (totalRooms - 1);

            if (progress < 0.30f)
            {
                return BiomeType.Ruined;
            }

            if (progress < 0.60f)
            {
                return BiomeType.Flooded;
            }

            return BiomeType.Volcanic;
        }
        
        private RoomData GetRoomAtPosition(Vector3Int pos)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                RoomData room = rooms[i];
                
                int startX = room.center.x - room.width / 2;
                int endX = startX + room.width - 1;
                
                int startY = room.center.y - room.height / 2;
                int endY = startY + room.height - 1;

                if (pos.x >= startX && pos.x <= endX && pos.y >= startY && pos.y <= endY)
                {
                    return room;
                }
            }

            return null;
        }

        private RoomData GetNearestRoomToPosition(Vector3Int pos)
        {
            RoomData nearestRoom = rooms[0];
            float shortestDistance = Vector3Int.Distance(pos, rooms[0].center);

            for (int i = 1; i < rooms.Count; i++)
            {
                float distanceToRoom = Vector3Int.Distance(pos, rooms[i].center);

                if (distanceToRoom < shortestDistance)
                {
                    shortestDistance = distanceToRoom;
                    nearestRoom = rooms[i];
                }
            }
            
            return nearestRoom;
        }
        
        
        
        
        
        //Decorations
        private void ApplyDecorations(System.Random rng)
        {
            BoundsInt bounds = wallsTilemap.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (collisionDecorationTilemap.HasTile(pos) || nonCollisionDecorationTilemap.HasTile(pos))
                    {
                        continue;
                    }
                    if (doorsTilemap.HasTile(pos))
                    {
                        continue;
                    }
                    if (IsWallTile(pos))
                    {
                        continue;
                    }
                    if (IsNearDoor(pos))
                    {
                        continue;
                    }


                    if (IsFloorTile(pos))
                    {
                        PlaceDecoration(pos, rng);
                    }
                }                
            }
        }

        private void PlaceDecoration(Vector3Int pos, System.Random rng)
        {
            RoomData room = GetRoomAtPosition(pos);
            if (room == null)
            {
                room = GetNearestRoomToPosition(pos);
            }

            bool isNearWall = IsNearWall(pos);

            double chance = rng.NextDouble();

            switch (room.biome)
            {
                case BiomeType.Ruined:
                    if (isNearWall && chance < 0.08)
                    {
                        collisionDecorationTilemap.SetTile(pos, dungeonSettings.BookshelfTile);
                        return;
                    }
                    if (chance < 0.10)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.CandlesTile);
                        return;
                    }
                    if (chance < 0.12)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.SkullTile);
                        return;
                    }
                    if (chance < 0.13)
                    {
                        collisionDecorationTilemap.SetTile(pos, dungeonSettings.TableTile);
                        return;
                    }
                    if (chance < 0.15)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.BloodTile);
                    }
                    break;
                
                case BiomeType.Flooded:
                    if (isNearWall && chance < 0.08)
                    {
                        collisionDecorationTilemap.SetTile(pos, dungeonSettings.BookshelfTile);
                        return;
                    }
                    if (isNearWall && chance < 0.13)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.CobwebTile);
                        return;
                    }
                    if (chance < 0.12)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.SkullTile);
                        return;
                    }
                    if (chance < 0.14)
                    {
                        collisionDecorationTilemap.SetTile(pos, dungeonSettings.TableTile);
                    }
                    break;
                
                case BiomeType.Volcanic:
                    if (chance < 0.10)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.HoleTile);
                        return;
                    }
                    if (chance < 0.12)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.SkullTile);
                        return;
                    }
                    if (chance < 0.15)
                    {
                        nonCollisionDecorationTilemap.SetTile(pos, dungeonSettings.BloodTile);
                    }
                    break;
            }
        }

        private bool IsFloorTile(Vector3Int pos)
        {
            TileBase tile = floorsTilemap.GetTile(pos);

            return tile == dungeonSettings.DryStoneTile ||
                   tile == dungeonSettings.CrackedStoneTile ||
                   tile == dungeonSettings.MossyStoneTile ||
                   tile == dungeonSettings.DampStoneTile ||
                   tile == dungeonSettings.VolanicRubbleTile;
        }
        
        private bool IsWallTile(Vector3Int pos)
        {
            TileBase tile = wallsTilemap.GetTile(pos);

            return tile == dungeonSettings.WallTile ||
                   tile == dungeonSettings.SecretWallTile;
        }

        private bool IsNearWall(Vector3Int pos)
        {
            return IsWallTile(pos + Vector3Int.up) ||
                   IsWallTile(pos + Vector3Int.down) ||
                   IsWallTile(pos + Vector3Int.left) ||
                   IsWallTile(pos + Vector3Int.right);
        }

        private bool IsNearDoor(Vector3Int pos)
        {
            int radius = 1;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector3Int checkPosition = new Vector3Int(pos.x + x, pos.y + y, 0);

                    if (doorsTilemap.HasTile(checkPosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        
        
        //Secret Room
        private void GenerateSecretSign(System.Random rng, Vector3Int ladderPosition)
        {
            RoomData ladderRoom = GetRoomAtPosition(ladderPosition);
            List<Vector3Int> validSignPositions = GetValidRoomFloorPositions(ladderRoom);

            Vector3Int signPosition = validSignPositions[rng.Next(validSignPositions.Count)];
            collisionDecorationTilemap.SetTile(signPosition, dungeonSettings.SignTile);
            
            Vector3 worldPosition = collisionDecorationTilemap.GetCellCenterWorld(signPosition);
            worldPosition.z = -4f;
            
            GameObject signObject = Instantiate(signPrefab, worldPosition, Quaternion.identity, transform);
            SignScript signScript =  signObject.GetComponent<SignScript>();
            signScript.SetSignUIManager(FindFirstObjectByType<UIScripts.SignUIManager>());
        }
        
        private List<Vector3Int> GetValidRoomFloorPositions(RoomData room)
        {
            List<Vector3Int> validPositions = new List<Vector3Int>();

            int startX = room.center.x - room.width / 2;
            int endX = startX + room.width - 1;
            
            int startY = room.center.y - room.height / 2;
            int endY = startY + room.height - 1;

            for (int x = startX + 1; x <= endX; x++)
            {
                for (int y = startY + 1; y <= endY; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (!IsFloorTile(pos))
                    {
                        continue;
                    }
                    if (IsNearDoor(pos))
                    {
                        continue;
                    }
                    if (collisionDecorationTilemap.HasTile(pos) || nonCollisionDecorationTilemap.HasTile(pos))
                    {
                        continue;
                    }
                    
                    validPositions.Add(pos);
                }
            }

            return validPositions;
        }
        
        private void GenerateSecretWall(System.Random rng)
        {
            List<Vector3Int> possibleWallPositions = new List<Vector3Int>();
            
            for (int i = 1; i < rooms.Count - 1; i++)
            {
                possibleWallPositions.AddRange(GetALlWallPositions(rooms[i]));
            }
            
            Vector3Int secretWallPos = possibleWallPositions[rng.Next(possibleWallPositions.Count)];
            dungeonLadderPosition = secretWallPos;
            
            LadderScript secretRoomLadderScript = secretRoomLadderObject.GetComponent<LadderScript>();
            Vector3 dungeonLadderWorldPosition = GetDungeonLadderExitWorldPosition(dungeonLadderPosition);
            secretRoomLadderScript.InitialiseLadder(dungeonLadderWorldPosition);
            
            wallsTilemap.SetTile(secretWallPos, dungeonSettings.SecretWallTile);
            
            Vector3 worldPosition = wallsTilemap.GetCellCenterWorld(secretWallPos);
            GameObject secretWallObject = Instantiate(secretWallPrefab, worldPosition, Quaternion.identity, transform);
            
            SecretWallScript secretWallScript = secretWallObject.GetComponent<SecretWallScript>();
            Vector3 secretRoomWorldPosition = collisionDecorationTilemap.GetCellCenterWorld(secretRoomLadderPosition) + new Vector3(0f, 1f, 0f);
            
            secretWallScript.InitialiseSecretWall(wallsTilemap, collisionDecorationTilemap, secretWallPos, dungeonSettings.LadderTile, ladderPrefab, secretRoomWorldPosition);
        }

        private Vector3 GetDungeonLadderExitWorldPosition(Vector3Int wallPosition)
        {
            Vector3Int[] checkDirections =
            {
                Vector3Int.up,
                Vector3Int.right,
                Vector3Int.down,
                Vector3Int.left
            };
            
            for (int i = 0; i < checkDirections.Length; i++)
            {
                Vector3Int checkPosition = wallPosition + checkDirections[i];

                if (!IsFloorTile(checkPosition))
                {
                    continue;
                }

                if (collisionDecorationTilemap.HasTile(checkPosition))
                {
                    continue;
                }
                
                Vector3 worldPosition = floorsTilemap.GetCellCenterWorld(checkPosition);
                worldPosition.z = -4f;
                return worldPosition;
            }

            return Vector3Int.zero;
        }

        private List<Vector3Int> GetALlWallPositions(RoomData room)
        {
            List<Vector3Int> possiblePositions = new List<Vector3Int>();

            int startX = room.center.x - room.width / 2;
            int endX = room.center.x + room.width / 2;
            
            int startY = room.center.y - room.height / 2;
            int endY = room.center.y + room.height / 2;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (wallsTilemap.GetTile(pos) != dungeonSettings.WallTile)
                    {
                        continue;
                    }

                    //corner walls
                    if ((x == startX && y == startY) || (x == startX && y == endY) || (x == endX && y == startY) || (x == endX && y == endY))
                    {
                        continue;
                    }
                    
                    //check that the wall actually touches a floor. this is important for circle and cross rooms
                    if (!TouchesFloor(pos))
                    {
                        continue;
                    }
                    
                    possiblePositions.Add(pos);
                }
            }
            
            return possiblePositions;
        }

        private bool TouchesFloor(Vector3Int pos)
        {
            return IsFloorTile(pos + Vector3Int.up) ||
                   IsFloorTile(pos + Vector3Int.down) ||
                   IsFloorTile(pos + Vector3Int.right) ||
                   IsFloorTile(pos + Vector3Int.left);
        }

        private void GenerateSecretRoom()
        {
            BoundsInt bounds = wallsTilemap.cellBounds;
            
            secretRoomCenter = new Vector3Int(bounds.xMin + 2 - 30, bounds.yMin, 0);   //30 is padding
            secretRoomLadderPosition = new Vector3Int(secretRoomCenter.x, secretRoomCenter.y - 3, 0);
            
            //now draw the room
            int width = 5;
            int height = 6;
            
            int startX = secretRoomCenter.x - width / 2;
            int startY = secretRoomCenter.y - height / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);

                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.WallTile);
                    }
                    else
                    {
                        floorsTilemap.SetTile(tilePos, dungeonSettings.DryStoneTile);
                    }
                }
            }
            
            collisionDecorationTilemap.SetTile(secretRoomCenter, dungeonSettings.ClosedChestTile);
            collisionDecorationTilemap.SetTile(secretRoomLadderPosition, dungeonSettings.LadderTile);
            
            Vector3 chestWorldPosition = collisionDecorationTilemap.GetCellCenterWorld(secretRoomCenter);
            GameObject chestObject = Instantiate(chestPrefab, chestWorldPosition, Quaternion.identity, transform);
            
            ChestScript chestScript = chestObject.GetComponent<ChestScript>();
            chestScript.InitialiseChest(collisionDecorationTilemap, secretRoomCenter, dungeonSettings);
            chestScript.SetRewardManager(FindFirstObjectByType<UIScripts.ChestRewardManager>());
            
            Vector3 ladderWorldPosition = collisionDecorationTilemap.GetCellCenterWorld(secretRoomLadderPosition);
            secretRoomLadderObject = Instantiate(ladderPrefab, ladderWorldPosition, Quaternion.identity, transform);
        }
        
        
        
        //Generate Pickups
        private void GeneratePickups(System.Random rng)
        {
            BoundsInt bounds = floorsTilemap.cellBounds;

            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y <= bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (!IsFloorTile(pos))
                    {
                        continue;
                    }
                    if (IsNearDoor(pos))
                    {
                        continue;
                    }
                    if (nonCollisionDecorationTilemap.HasTile(pos) || collisionDecorationTilemap.HasTile(pos))
                    {
                        continue;
                    }

                    
                    Vector3 worldPosition = floorsTilemap.CellToWorld(pos);
                    worldPosition.x += 0.5f;
                    worldPosition.y += 0.5f;
                    worldPosition.z -= 4f;
                    double spawnChance = rng.NextDouble();
                    if (spawnChance < 0.0025)
                    {
                        Instantiate(medkitPickupPrefab, worldPosition, Quaternion.identity, transform);
                        continue;
                    }
                    if (spawnChance < 0.015)
                    {
                        Instantiate(ammoPickupPrefab, worldPosition, Quaternion.identity, transform);
                    }
                }
            }
        }
        
        
        
        //Generate enemies
        private void GenerateEnemySpawnPoints(System.Random rng)
        {
            for (int roomIndex = 1; roomIndex < rooms.Count - 1; roomIndex++)
            {
                RoomData room = rooms[roomIndex];
                List<Vector3Int> validPositions = GetValidRoomFloorPositions(room);
                
                int roomArea = room.width * room.height;
                int baseSpawnCount;

                if (roomArea < 50)
                {
                    baseSpawnCount = 1;
                }
                else if (roomArea < 90)
                {
                    baseSpawnCount = 2;
                }
                else
                {
                    baseSpawnCount = 3;
                }
                
                //add more enemies the further in the dungeon level the player is
                int distanceBonus = Mathf.Clamp(roomIndex / 2, 0, 2);
                int spawnCount = Mathf.Min(baseSpawnCount + distanceBonus, validPositions.Count);

                for (int i = 0; i < spawnCount; i++)
                {
                    int randomIndex = rng.Next(validPositions.Count);
                    Vector3Int chosenPosition = validPositions[randomIndex];

                    EnemyType chosenEnemyType = GetRandomEnemyType(rng);
                    enemySpawnPoints.Add(new EnemySpawnData(chosenPosition, roomIndex, chosenEnemyType));
                    GameObject prefabToSpawn = GetEnemyPrefab(chosenEnemyType);

                    Vector3 worldPosition = floorsTilemap.GetCellCenterWorld(chosenPosition);
                    worldPosition.z = -4f;
                    
                    Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, transform);
                }
            }
        }
        
        private EnemyType GetRandomEnemyType(System.Random rng)
        {
            int randomNum = rng.Next(0, 3);

            switch (randomNum)
            {
                case 0:
                    return EnemyType.Zombie;
                case 1:
                    return EnemyType.Skeleton;
                default:
                    return EnemyType.Spider;
            }
        }

        private GameObject GetEnemyPrefab(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Zombie:
                    return zombieEnemyPrefab;
                case EnemyType.Skeleton:
                    return skeletonEnemyPrefab;
                default:
                    return spiderEnemyPrefab;
            }
        }
        
        
        
        //Background
        private void ApplyBackgroundTiles()
        {
            BoundsInt bounds = wallsTilemap.cellBounds;

            int padding = 9;

            for (int x = bounds.xMin - padding; x < bounds.xMax + padding; x++)
            {
                for (int y = bounds.yMin - padding; y < bounds.yMax + padding; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (wallsTilemap.HasTile(pos) == false)
                    {
                        backgroundTilemap.SetTile(new Vector3Int(x, y, 0), dungeonSettings.BackgroundTile);
                    }
                }
            }
        }
        
        
        
        
        
        
        //Reset button function
        [Button("Reset Tilemaps")]
        public void ResetGrid()
        {
            floorsTilemap.ClearAllTiles();
            wallsTilemap.ClearAllTiles();
            doorsTilemap.ClearAllTiles();
            collisionDecorationTilemap.ClearAllTiles();
            nonCollisionDecorationTilemap.ClearAllTiles();
            backgroundTilemap.ClearAllTiles();
            
            rooms.Clear(); 
            enemySpawnPoints.Clear();
            
            //destroy the old spawned gameobjects
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}