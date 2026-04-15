using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;
using DungeonLayoutGeneration.Settings;
using System.Collections.Generic;
using GameManagerScripts;
using InteractableScripts;
using CorridorTrapScripts.ArrowTrap;
using CorridorTrapScripts.FlameJetsTrap;
using CorridorTrapScripts.SpikesTrap;

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

    public enum CorridorTrapType
    {
        PressurePlateArrow,
        FlameJet,
        Spikes
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
        [SerializeField] private Tilemap ladderTilemap;
        [SerializeField] private Tilemap collisionDecorationTilemap; 
        [SerializeField] private Tilemap nonCollisionDecorationTilemap;
        [SerializeField] private Tilemap backgroundTilemap;
        [SerializeField] private Tilemap vegetationTilemap;

        private readonly HashSet<Vector3Int> corridorFloorPositions = new HashSet<Vector3Int>();
        private readonly HashSet<Vector3Int> trapPositions = new HashSet<Vector3Int>();
        private readonly List<CorridorArrowData> corridorArrowTraps = new List<CorridorArrowData>();
        [SerializeField] private GameObject pressurePlatePrefab;
        private readonly List<SpikesData> spikesTraps = new List<SpikesData>();
        [SerializeField] private GameObject spikesTrapPrefab;
        private readonly List<FlameJetData> flameJetsTraps = new List<FlameJetData>();
        [SerializeField] private GameObject flameJetPrefab;
        
        
        private int numberOfRooms;
        private int distance;
        
        private readonly List<RoomData> rooms = new List<RoomData>();

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
        [SerializeField] private GameObject exitDoorPrefab;

        [SerializeField] private GameObject zombieEnemyPrefab;
        [SerializeField] private GameObject skeletonEnemyPrefab;
        [SerializeField] private GameObject spiderEnemyPrefab;
        [SerializeField] RoomCombatManager roomCombatManager;


        public void SetSeed(int newSeed)
        {
            dungeonSettings.Seed = newSeed;
        }
        
        [Button("Generate Platformer")]
        public void GenerateDungeon()
        {
            //0. Reset the previous tilemaps and List
            ResetGrid();
            
            
            //1. Initialize Random/Seed and Room Combat Manager
            System.Random rng = new System.Random(dungeonSettings.Seed);
            roomCombatManager.ResetRoomData();
            
            //2. Generate Rooms              
            GenerateRooms(rng);
            
            //3. Generate Start and End Room  
            GenerateStartRoom();
            GenerateEndRoom();
            
            //4. Add Corridors
            GenerateCorridorFloors();
            GenerateCorridorWalls();
            
            //5. Add Traps to the Corridors
            GenerateCorridorTraps(rng);
            SpawnArrowTraps();
            SpawnSpikesTraps();
            SpawnFlameJetsTraps();
            
            //6. Add Floor Variation
            ApplyFloorVariation();
            
            //7. Add Secret Room
            GenerateSecretRoom();
            GenerateSecretWall(rng);
            GenerateSecretSign(rng);
            
            //8. Add Vegetation & Organic Life
            GenerateWallVines(rng);
            GenerateTreeStumps(rng);
            GenerateMushrooms(rng);
            GenerateFossils(rng);
            
            //9. Add decorations
            ApplyDecorations(rng);
            
            //10. Spawn Pickups
            GeneratePickups(rng);
            
            
            //11. Generate enemy spawn points
            GenerateEnemySpawnPoints(rng);
            
            
            //12. Add Background
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
                    
                    if (x == 0 && y == height - 1)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.TopLeftCornerWallTile);
                    }
                    else if (x == width - 1 && y == height - 1)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.TopRightCornerWallTile);
                    }
                    else if (x == 0 && y == 0)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.BottomLeftCornerWallTile);
                    }
                    else if (x == width - 1 && y == 0)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.BottomRightCornerWallTile);
                    }
                    
                    else if (x == 0)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.LeftSideWallTile);
                    }
                    else if (x == width - 1)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.RightSideWallTile);
                    }
                    else if (y == height - 1)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.TopWallTile);
                    } 
                    else if (y == 0)
                    {
                        wallsTilemap.SetTile(tilePos, dungeonSettings.BottomWallTile);
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
                            wallsTilemap.SetTile(tilePos, dungeonSettings.BottomWallTile);
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
                        wallsTilemap.SetTile(tilePos, dungeonSettings.BottomWallTile);
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
            RoomData lastRoom =  rooms[^1];   //last element in array
            
            int endRoomWidth = 9;
            int endRoomHeight = 7;
            
            Vector3Int endRoomCenter = new Vector3Int(lastRoom.center.x + lastRoom.width / 2 + endRoomWidth / 2 + 10, lastRoom.center.y, 0);
            
            DrawRectangleRoom(endRoomCenter, endRoomWidth, endRoomHeight);
            
            Vector3Int doorPos = new Vector3Int(endRoomCenter.x, endRoomCenter.y + endRoomHeight / 2, 0);
            doorsTilemap.SetTile(doorPos, dungeonSettings.ExitDoorTile);

            Vector3 worldPosition = doorsTilemap.GetCellCenterWorld(doorPos);
            worldPosition.z = -4f;
            Instantiate(exitDoorPrefab, worldPosition, Quaternion.identity, transform);
            
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
                    corridorFloorPositions.Add(floorPos);
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
                    corridorFloorPositions.Add(floorPos);
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

                    bool floorUp = floorsTilemap.HasTile(pos + Vector3Int.up);
                    bool floorDown = floorsTilemap.HasTile(pos + Vector3Int.down);
                    bool floorLeft = floorsTilemap.HasTile(pos + Vector3Int.left);
                    bool floorRight = floorsTilemap.HasTile(pos + Vector3Int.right);


                    if (!floorUp && !floorDown && !floorLeft && !floorRight)
                    {
                        continue;
                    }
                    
                    if (floorDown)
                    {
                        wallsTilemap.SetTile(pos, dungeonSettings.TopWallTile);
                    }
                    else if (floorUp)
                    {
                        wallsTilemap.SetTile(pos, dungeonSettings.BottomWallTile);
                    }
                    else if (floorRight)
                    {
                        wallsTilemap.SetTile(pos, dungeonSettings.LeftSideWallTile);
                    }
                    else //floorLeft
                    {
                        wallsTilemap.SetTile(pos, dungeonSettings.RightSideWallTile);
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
                
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY - 1, 0), dungeonSettings.RightSideWallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), null);
                doorsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), dungeonSettings.DoorTile);
                SpawnDoorObject(new Vector3Int(doorX, doorY, 0));
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY + 1, 0), dungeonSettings.RightSideWallTile);
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
                
                wallsTilemap.SetTile(new Vector3Int(doorX - 1, doorY, 0), dungeonSettings.BottomWallTile);
                wallsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), null);
                doorsTilemap.SetTile(new Vector3Int(doorX, doorY, 0), dungeonSettings.DoorTile);
                SpawnDoorObject(new Vector3Int(doorX, doorY, 0));
                wallsTilemap.SetTile(new Vector3Int(doorX + 1, doorY, 0), dungeonSettings.BottomWallTile);
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

            float offsetX = (dungeonSettings.Seed % 10000) * 0.67f;
            float offsetY = (dungeonSettings.Seed % 10000) * 0.69f;

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
                return dungeonSettings.VolcanicRubbleTile;
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
            foreach (RoomData room in rooms)
            {
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
                    if (IsTrap(pos))
                    {
                        continue;
                    }
                    if (IsNearLadder(pos))
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
            if (room == null)
            {
                return;
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
                   tile == dungeonSettings.VolcanicRubbleTile;
        }
        
        private bool IsWallTile(Vector3Int pos)
        {
            TileBase tile = wallsTilemap.GetTile(pos);

            return tile == dungeonSettings.TopLeftCornerWallTile ||
                   tile == dungeonSettings.TopRightCornerWallTile ||
                   tile == dungeonSettings.BottomLeftCornerWallTile ||
                   tile == dungeonSettings.BottomRightCornerWallTile ||
                   tile == dungeonSettings.TopWallTile ||
                   tile == dungeonSettings.BottomWallTile ||
                   tile == dungeonSettings.LeftSideWallTile ||
                   tile == dungeonSettings.RightSideWallTile ||
                   tile == dungeonSettings.SecretWallTile ||
                   tile == dungeonSettings.ExitDoorTile ||
                   tile == dungeonSettings.TrapLauncherTile;
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
        private void GenerateSecretSign(System.Random rng)
        {
            RoomData ladderRoom = GetRoomAtPosition(dungeonLadderPosition);
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

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (!IsFloorTile(pos))
                    {
                        continue;
                    }
                    if (IsNearWall(pos))
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
                    if (IsTrap(pos))
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
            Vector3 dungeonLadderWorldPosition = GetDungeonLadderExitWorldPosition(secretWallPos);
            secretRoomLadderScript.InitialiseLadder(dungeonLadderWorldPosition);
            
            wallsTilemap.SetTile(secretWallPos, dungeonSettings.SecretWallTile);
            
            Vector3 worldPosition = wallsTilemap.GetCellCenterWorld(secretWallPos);
            GameObject secretWallObject = Instantiate(secretWallPrefab, worldPosition, Quaternion.identity, transform);
            
            SecretWallScript secretWallScript = secretWallObject.GetComponent<SecretWallScript>();
            Vector3 secretRoomWorldPosition = ladderTilemap.GetCellCenterWorld(secretRoomLadderPosition) + new Vector3(0f, 1f, 0f);
            
            secretWallScript.InitialiseSecretWall(wallsTilemap, ladderTilemap, secretWallPos, dungeonSettings.LadderTile, ladderPrefab, secretRoomWorldPosition);
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
            
            foreach (Vector3Int direction in checkDirections)
            {
                Vector3Int checkPosition = wallPosition + direction;

                if (!IsFloorTile(checkPosition))
                {
                    continue;
                }

                if (collisionDecorationTilemap.HasTile(checkPosition))
                {
                    continue;
                }
                
                dungeonLadderPosition = checkPosition;
                
                Vector3 worldPosition = floorsTilemap.GetCellCenterWorld(checkPosition);
                worldPosition.z = -4f;
                return worldPosition;
            }

            return Vector3Int.zero;
        }

        private bool IsNearLadder(Vector3Int pos)
        {
            return pos == dungeonLadderPosition ||
                   pos == secretRoomLadderPosition ||
                   pos == dungeonLadderPosition + Vector3Int.up ||
                   pos == dungeonLadderPosition + Vector3Int.down ||
                   pos == dungeonLadderPosition + Vector3Int.left ||
                   pos == dungeonLadderPosition + Vector3Int.right ||
                   pos == secretRoomLadderPosition + Vector3Int.up ||
                   pos == secretRoomLadderPosition + Vector3Int.down ||
                   pos == secretRoomLadderPosition + Vector3Int.left ||
                   pos == secretRoomLadderPosition + Vector3Int.right;

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

                    if (!IsWallTile(pos))
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
            
            DrawRectangleRoom(secretRoomCenter, width, height);
            
            collisionDecorationTilemap.SetTile(secretRoomCenter, dungeonSettings.ClosedChestTile);
            ladderTilemap.SetTile(secretRoomLadderPosition, dungeonSettings.LadderTile);
            
            Vector3 chestWorldPosition = collisionDecorationTilemap.GetCellCenterWorld(secretRoomCenter);
            GameObject chestObject = Instantiate(chestPrefab, chestWorldPosition, Quaternion.identity, transform);
            
            ChestScript chestScript = chestObject.GetComponent<ChestScript>();
            chestScript.InitialiseChest(collisionDecorationTilemap, secretRoomCenter, dungeonSettings);
            chestScript.SetRewardManager(FindFirstObjectByType<UIScripts.ChestRewardManager>());
            
            Vector3 ladderWorldPosition = ladderTilemap.GetCellCenterWorld(secretRoomLadderPosition);
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
                    if (IsTrap(pos))
                    {
                        continue;
                    }
                    if (GetRoomAtPosition(pos) == null)
                    {
                        continue;
                    }

                    
                    Vector3 worldPosition = floorsTilemap.CellToWorld(pos);
                    worldPosition.x += 0.5f;
                    worldPosition.y += 0.5f;
                    worldPosition.z -= 4f;
                    double spawnChance = rng.NextDouble();
                    if (spawnChance < 0.0075)
                    {
                        Instantiate(medkitPickupPrefab, worldPosition, Quaternion.identity, transform);
                        continue;
                    }
                    if (spawnChance < 0.025)
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
                    validPositions.RemoveAt(randomIndex);

                    EnemyType chosenEnemyType = GetRandomEnemyType(rng);
                    GameObject prefabToSpawn = GetEnemyPrefab(chosenEnemyType);

                    Vector3 worldPosition = floorsTilemap.GetCellCenterWorld(chosenPosition);
                    worldPosition.z = -4f;
                    
                    GameObject enemyObject = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, transform);
                    EnemyScripts.EnemyScript enemyScript = enemyObject.GetComponent<EnemyScripts.EnemyScript>();
                    enemyScript.InitialiseEnemy(roomIndex, roomCombatManager);
                    roomCombatManager.InitialiseEnemy(roomIndex);
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
        
        
        
        //Generate traps
        private void GenerateCorridorTraps(System.Random rng)
        {
            corridorArrowTraps.Clear();
            spikesTraps.Clear();
            flameJetsTraps.Clear();

            for (int i = 0; i < rooms.Count - 1; i++)
            {
                RoomData startRoom = rooms[i];
                RoomData endRoom = rooms[i + 1];
                
                Vector3Int start = startRoom.center;
                Vector3Int end = endRoom.center;
                
                List <Vector3Int> validPositions = new List<Vector3Int>();

                for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
                {
                    for (int offset = -1; offset <= 1; offset++)
                    {
                        Vector3Int pos = new Vector3Int(x, start.y + offset, 0);
                        
                        if (!IsFloorTile(pos))
                        {
                            continue;
                        }
                        if (!IsCorridorFloor(pos))
                        {
                            continue;
                        }
                        if (GetRoomAtPosition(pos) != null)
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
                        if (IsTrap(pos))
                        {
                            continue;
                        }
                        if (validPositions.Contains(pos))
                        {
                            continue;
                        }
                        
                        validPositions.Add(pos);
                    }
                }
                for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
                {
                    for (int offset = -1; offset <= 1; offset++)
                    {
                        Vector3Int pos = new Vector3Int(end.x + offset, y, 0);

                        if (!IsFloorTile(pos))
                        {
                            continue;
                        }
                        if (!IsMiddleOfVerticalCorridor(pos) && !IsMiddleOfHorizontalCorridor(pos))
                        {
                            continue;
                        }
                        if (GetRoomAtPosition(pos) != null)
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
                        if (IsTrap(pos))
                        {
                            continue;
                        }
                        if (validPositions.Contains(pos))
                        {
                            continue;
                        }

                        validPositions.Add(pos);
                    }
                }

                int trapCount = 2;
                if (validPositions.Count >= 12)
                {
                    trapCount = 3;
                }
                if (validPositions.Count >= 22)
                {
                    trapCount = 4;
                }
                
                for (int trapIndex = 0; trapIndex < trapCount; trapIndex++)
                {
                    int randomIndex = rng.Next(validPositions.Count);
                    Vector3Int chosenPosition = validPositions[randomIndex];
                    validPositions.RemoveAt(randomIndex);
                    
                    CorridorTrapType trapType = (CorridorTrapType)rng.Next(0, 3);

                    if (trapType == CorridorTrapType.Spikes)
                    {
                        spikesTraps.Add(new SpikesData(chosenPosition));
                        trapPositions.Add(chosenPosition);
                        continue;
                    }

                    Vector3Int wallTrapPosition = Vector3Int.zero;
                    Vector2Int trapDirection = Vector2Int.zero;

                    if (IsMiddleOfVerticalCorridor(chosenPosition))
                    {
                        bool useLeftWall = rng.Next(0, 2) == 0;

                        if (useLeftWall)
                        {
                            wallTrapPosition = chosenPosition + Vector3Int.left * 2;
                            trapDirection = Vector2Int.right;
                        }
                        else
                        {
                            wallTrapPosition = chosenPosition + Vector3Int.right * 2;
                            trapDirection = Vector2Int.left;
                        }
                    }
                    else if (IsMiddleOfHorizontalCorridor(chosenPosition))
                    {
                        bool useTopWall = rng.Next(0, 2) == 0;

                        if (useTopWall)
                        {
                            wallTrapPosition = chosenPosition + Vector3Int.up * 2;
                            trapDirection = Vector2Int.down;
                        }
                        else
                        {
                            wallTrapPosition = chosenPosition + Vector3Int.down * 2;
                            trapDirection = Vector2Int.up;
                        }
                    }

                    if (!IsWallTile(wallTrapPosition) || IsTrap(wallTrapPosition))
                    {
                        spikesTraps.Add(new SpikesData(chosenPosition));
                        trapPositions.Add(chosenPosition);
                        continue;
                    }

                    if (trapType == CorridorTrapType.PressurePlateArrow)
                    {
                        corridorArrowTraps.Add(new CorridorArrowData(chosenPosition, wallTrapPosition, trapDirection));
                        trapPositions.Add(chosenPosition);
                        trapPositions.Add(wallTrapPosition);
                    }
                    else  //flame jets
                    {
                        flameJetsTraps.Add(new FlameJetData(wallTrapPosition, trapDirection));
                        trapPositions.Add(wallTrapPosition);
                        
                        Vector3Int flameFrontTile = wallTrapPosition + new Vector3Int(trapDirection.x, trapDirection.y, 0);
                        trapPositions.Add(flameFrontTile);
                    }
                }
            }
        }
        
        private bool IsCorridorFloor(Vector3Int pos)
        {
            return corridorFloorPositions.Contains(pos);
        }

        private bool IsMiddleOfHorizontalCorridor(Vector3Int pos)
        {
            return IsCorridorFloor(pos + Vector3Int.left) &&
                   IsCorridorFloor(pos + Vector3Int.right) &&
                   IsWallTile(pos + Vector3Int.up * 2) &&
                   IsWallTile(pos + Vector3Int.down * 2);
        }

        private bool IsMiddleOfVerticalCorridor(Vector3Int pos)
        {
            return IsCorridorFloor(pos + Vector3Int.up) &&
                   IsCorridorFloor(pos + Vector3Int.down) &&
                   IsWallTile(pos + Vector3Int.left * 2) &&
                   IsWallTile(pos + Vector3Int.right * 2);
        }
        
        private bool IsTrap(Vector3Int pos)
        {
            return trapPositions.Contains(pos);
        }
        
        
        
        private void SpawnSpikesTraps()
        {
            foreach (SpikesData trap in spikesTraps)
            {
                Vector3 worldPosition = floorsTilemap.GetCellCenterWorld(trap.spikePosition);
                worldPosition.z = -4f;
                Instantiate(spikesTrapPrefab, worldPosition, Quaternion.identity, transform);
            }
        }

        private void SpawnArrowTraps()
        {
            foreach (CorridorArrowData trap in corridorArrowTraps)
            {
                wallsTilemap.SetTile(trap.launcherPosition, dungeonSettings.TrapLauncherTile);
                
                Vector3 plateWorldPosition = floorsTilemap.GetCellCenterWorld(trap.platePosition);
                plateWorldPosition.z = -4f;
                
                GameObject pressurePlateObject = Instantiate(pressurePlatePrefab, plateWorldPosition, Quaternion.identity, transform);
                
                PressurePlateTrap pressurePlateTrap = pressurePlateObject.GetComponent<PressurePlateTrap>();
                pressurePlateTrap.InitialiseTrap(trap.launcherPosition, trap.fireDirection);
            }
        }

        private void SpawnFlameJetsTraps()
        {
            foreach (FlameJetData trap in flameJetsTraps)
            {
                wallsTilemap.SetTile(trap.flamePosition, dungeonSettings.TrapLauncherTile);
                
                Vector3 worldPosition = wallsTilemap.GetCellCenterWorld(trap.flamePosition);
                worldPosition.z = -4f;
                
                GameObject flameJetObject = Instantiate(flameJetPrefab, worldPosition, Quaternion.identity, transform);
                float angle = 0f;

                if (trap.flameDirection == Vector2Int.right)
                {
                    angle = 0f;
                }
                else if (trap.flameDirection == Vector2Int.up)
                {
                    angle = 90f;
                }
                else if (trap.flameDirection == Vector2Int.left)
                {
                    angle = 180f;
                    SpriteRenderer spriteRenderer = flameJetObject.GetComponentInChildren<SpriteRenderer>();
                    spriteRenderer.flipY = true;
                }
                else if (trap.flameDirection == Vector2Int.down)
                {
                    angle = -90f;
                }

                flameJetObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
        
        
        
        
        //Vegetation
        private void GenerateWallVines(System.Random rng)
        {
            BoundsInt bounds = wallsTilemap.cellBounds;
            float depthFactor = Mathf.Clamp01((RunData.FloorsReached - 1) / 5f);

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int wallPos = new Vector3Int(x, y, 0);
                    TileBase wallTile = wallsTilemap.GetTile(wallPos);

                    if (!IsWallTile(wallPos))
                    {
                        continue;
                    }
                    if (doorsTilemap.HasTile(wallPos))
                    {
                        continue;
                    }
                    if (vegetationTilemap.HasTile(wallPos))
                    {
                        continue;
                    }
                    if (wallsTilemap.GetTile(wallPos) == dungeonSettings.SecretWallTile || wallsTilemap.GetTile(wallPos) == dungeonSettings.TrapLauncherTile)
                    {
                        continue;
                    }
                    if (IsTrap(wallPos) || IsNearDoor(wallPos))
                    {
                        continue;
                    }
                    if (ladderTilemap.HasTile(wallPos))
                    {
                        continue;
                    }

                    Vector3Int growthDirection = Vector3Int.zero;
                    if (wallTile == dungeonSettings.TopWallTile || wallTile == dungeonSettings.BottomWallTile || wallTile == dungeonSettings.TopLeftCornerWallTile || wallTile == dungeonSettings.TopRightCornerWallTile || wallTile == dungeonSettings.BottomLeftCornerWallTile || wallTile == dungeonSettings.BottomRightCornerWallTile)
                    {
                        if (rng.NextDouble() < 0.5f)
                        {
                            growthDirection = Vector3Int.left;
                        }
                        else
                        {
                            growthDirection = Vector3Int.right;
                        }
                    }
                    if (wallTile == dungeonSettings.LeftSideWallTile || wallTile == dungeonSettings.RightSideWallTile)
                    {
                        if (rng.NextDouble() < 0.5f)
                        {
                            growthDirection = Vector3Int.down;
                        }
                        else
                        {
                            growthDirection = Vector3Int.up;
                        }
                    }
                    

                    RoomData room = GetNearestRoomToPosition(wallPos);

                    float vineChance = 0f;
                    switch (room.biome)
                    {
                        case BiomeType.Ruined:
                            vineChance = 0.03f;
                            break;
                        case BiomeType.Flooded:
                            vineChance = 0.07f;
                            break;
                        case BiomeType.Volcanic:
                            vineChance = 0.01f;
                            break;
                    }

                    //deeper floors = more vine growth
                    vineChance += Mathf.Lerp(0f, 0.1f, depthFactor);

                    if (rng.NextDouble() > vineChance)
                    {
                        continue;
                    }
                    
                    
                    
                    float offsetX = (dungeonSettings.Seed % 10000) * 0.3f;
                    float offsetY = (dungeonSettings.Seed % 10000) * 0.4f;

                    float sampleX = wallPos.x * 0.12f + offsetX;
                    float sampleY = wallPos.y * 0.12f + offsetY;

                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                    if (noiseValue < 0.45f)
                    {
                        continue;
                    }
                        
                        
                    

                    int maxLength;
                    if (room.biome == BiomeType.Flooded)
                    {
                        maxLength = 5;
                    }
                    else
                    {
                        maxLength = 4;
                    }

                    maxLength += Mathf.RoundToInt(depthFactor * 3);
                        
                    int vineLength = rng.Next(1, maxLength + 1);
                    
                    Vector3Int currentPos = wallPos;
                    Vector3Int currentDirection = growthDirection;
                        
                    for (int i = 0; i < vineLength; i++)
                    {
                        if (i > 0)
                        {
                            Vector3Int nextPos = currentPos + currentDirection;

                            if (!CanPlaceVineAt(nextPos))
                            {
                                Vector3Int turnedDirection = GetNewVineDirection(currentDirection, currentPos, rng);
                                currentDirection = turnedDirection;
                                nextPos = currentPos + currentDirection;

                                if (!CanPlaceVineAt(nextPos))
                                {
                                    break;
                                }
                            }

                            currentPos = nextPos;
                        }

                        if (!CanPlaceVineAt(currentPos))
                        {
                            break;
                        }
                        
                        vegetationTilemap.SetTile(currentPos, dungeonSettings.VineTile);
                    }
                }
            }
        }

        private bool CanPlaceVineAt(Vector3Int pos)
        {
            if (!wallsTilemap.HasTile(pos))
            {
                return false;
            }
            if (doorsTilemap.HasTile(pos))
            {
                return false;
            }
            if (IsTrap(pos))
            {
                return false;
            }
            if (vegetationTilemap.HasTile(pos))
            {
                return false;
            }
            if (ladderTilemap.HasTile(pos))
            {
                return false;
            }
            if (pos == dungeonLadderPosition || pos == secretRoomLadderPosition)
            {
                return false;
            }

            return true;
        }

        private Vector3Int GetNewVineDirection(Vector3Int currentDirection, Vector3Int currentPos, System.Random rng)
        {
            Vector3Int firstOption;
            Vector3Int secondOption;

            if (currentDirection == Vector3Int.left || currentDirection == Vector3Int.right)
            {
                if (rng.NextDouble() < 0.5f)
                {
                    firstOption = Vector3Int.up;
                    secondOption = Vector3Int.down;
                }
                else
                {
                    firstOption = Vector3Int.down;
                    secondOption = Vector3Int.up;
                }
            }
            else
            {
                if (rng.NextDouble() < 0.5f)
                {
                    firstOption = Vector3Int.left;
                    secondOption = Vector3Int.right;
                }
                else
                {
                    firstOption = Vector3Int.right;
                    secondOption = Vector3Int.left;
                }
            }

            if (CanPlaceVineAt(currentPos + firstOption))
            {
                return firstOption;
            }

            if (CanPlaceVineAt(currentPos + secondOption))
            {
                return secondOption;
            }

            return Vector3Int.zero;
        }
        
        
        
        
        private void GenerateTreeStumps(System.Random rng)
        {
            float depthFactor = Mathf.Clamp01((RunData.FloorsReached - 1) / 5f);

            for (int roomIndex = 1; roomIndex < rooms.Count - 1; roomIndex++)
            {
                RoomData room = rooms[roomIndex];

                if (room.biome == BiomeType.Volcanic)
                {
                    continue;
                }

                List<Vector3Int> validPositions = new List<Vector3Int>();
                int startX = room.center.x - room.width / 2;
                int endX = startX + room.width - 1;
                int startY = room.center.y - room.height / 2;
                int endY = startY + room.height - 1;

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
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
                        if (IsNearLadder(pos))
                        {
                            continue;
                        }
                        if (IsTrap(pos))
                        {
                            continue;
                        }
                        if (collisionDecorationTilemap.HasTile(pos) || nonCollisionDecorationTilemap.HasTile(pos))
                        {
                            continue;
                        }
                        if (vegetationTilemap.HasTile(pos))
                        {
                            continue;
                        }
                        
                        validPositions.Add(pos);
                    }
                }

                float stumpChance;
                if (room.biome == BiomeType.Ruined)
                {
                    stumpChance = 0.05f;
                }
                else if (room.biome == BiomeType.Flooded)
                {
                    stumpChance = 0.1f;
                }
                else
                {
                    stumpChance = 0f;
                }

                stumpChance += Mathf.Lerp(0f, 0.12f, depthFactor);

                if (rng.NextDouble() > stumpChance)
                {
                    continue;
                }

                
                Vector3Int center = validPositions[rng.Next(validPositions.Count)];
                int clusterSize = rng.Next(2, 5); //2-4 stumps
                for (int i = 0; i < clusterSize; i++)
                {
                    Vector3Int offset = new Vector3Int(
                        rng.Next(-1, 2),
                        rng.Next(-1, 2),
                        0
                    );

                    Vector3Int pos = center + offset;
                    if (!validPositions.Contains(pos))
                    {
                        continue;
                    }
                    if (collisionDecorationTilemap.HasTile(pos) || nonCollisionDecorationTilemap.HasTile(pos) || vegetationTilemap.HasTile(pos))
                    {
                        continue;
                    }
                    
                    collisionDecorationTilemap.SetTile(pos, dungeonSettings.TreeStumpTile);
                }
            }
        }
        
        
        
        

        private void GenerateMushrooms(System.Random rng)
        {
            float depthFactor = Mathf.Clamp01((RunData.FloorsReached - 1) / 5f);

            for (int roomIndex = 1; roomIndex < rooms.Count - 1; roomIndex++)
            {
                RoomData room = rooms[roomIndex];

                if (room.biome == BiomeType.Volcanic)
                {
                    continue;
                }
                
                int startX = room.center.x - room.width / 2;
                int endX = startX + room.width - 1;
                int startY = room.center.y - room.height / 2;
                int endY = startY + room.height - 1;

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);

                        if (!IsFloorTile(pos))
                        {
                            continue;
                        }
                        if (IsTrap(pos))
                        {
                            continue;
                        }
                        if (collisionDecorationTilemap.HasTile(pos) || nonCollisionDecorationTilemap.HasTile(pos))
                        {
                            continue;
                        }
                        if (vegetationTilemap.HasTile(pos))
                        {
                            continue;
                        }

                        
                        float mushroomChance = 0f;
                        if (room.biome == BiomeType.Ruined)
                        {
                            mushroomChance = 0.015f;
                        }
                        if (room.biome == BiomeType.Flooded)
                        {
                            mushroomChance = 0.04f;
                        }

                        if (IsNearWall(pos))   //extra boost near walls because of moisture nears walls
                        {
                            mushroomChance *= 2f;
                        }

                        if (IsNearCorner(pos))
                        {
                            mushroomChance *= 1.5f;
                        }
                        
                        mushroomChance += Mathf.Lerp(0f, 0.05f, depthFactor);
                        
                        if (rng.NextDouble() > mushroomChance)
                        {
                            continue;
                        }


                        int clusterSize = rng.Next(2, 10);

                        for (int i = 0; i < clusterSize; i++)
                        {
                            Vector3Int offset = new Vector3Int(
                                rng.Next(-1, 2),
                                rng.Next(-1, 2),
                                0
                            );
                            Vector3Int spawnPos = pos + offset;

                            if (!IsFloorTile(spawnPos))
                            {
                                continue;
                            }
                            if (collisionDecorationTilemap.HasTile(spawnPos) || nonCollisionDecorationTilemap.HasTile(spawnPos) || vegetationTilemap.HasTile(spawnPos))
                            {
                                continue;
                            }
                            if (IsTrap(spawnPos))
                            {
                                continue;
                            }
                            
                            vegetationTilemap.SetTile(spawnPos, dungeonSettings.MushroomTile);
                        }
                    }
                }
            }
        }

        private bool IsNearCorner(Vector3Int pos)
        {
            bool wallUp = IsWallTile(pos + Vector3Int.up);
            bool wallDown = IsWallTile(pos + Vector3Int.down);
            bool wallLeft = IsWallTile(pos + Vector3Int.left);
            bool wallRight = IsWallTile(pos + Vector3Int.right);

            return (wallUp && wallLeft) ||
                   (wallUp && wallRight) ||
                   (wallDown && wallLeft) ||
                   (wallDown && wallRight);
        }
        
        
        

        private void GenerateFossils(System.Random rng)
        {
            float depthFactor = Mathf.Clamp01((RunData.FloorsReached - 1) / 5f);

            for (int roomIndex = 1; roomIndex < rooms.Count - 1; roomIndex++)
            {
                RoomData room = rooms[roomIndex];

                if (room.biome != BiomeType.Volcanic)
                {
                    continue;
                }

                int startX = room.center.x - room.width / 2;
                int endX = startX + room.width - 1;
                int startY = room.center.y - room.height / 2;
                int endY = startY + room.height - 1;

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        
                        TileBase tile = floorsTilemap.GetTile(pos);

                        if (tile != dungeonSettings.VolcanicRubbleTile)
                        {
                            continue;
                        }
                        if (collisionDecorationTilemap.HasTile(pos) || nonCollisionDecorationTilemap.HasTile(pos) || vegetationTilemap.HasTile(pos))
                        {
                            continue;
                        }
                        if (IsTrap(pos))
                        {
                            continue;
                        }
                        if (IsNearDoor(pos) || IsNearLadder(pos))
                        {
                            continue;
                        }

                        float fossilChance = 0.02f;
                        if (IsNearLava(pos))
                        {
                            fossilChance *= 3f;    //give big boost if near lava
                        }
                        
                        fossilChance += Mathf.Lerp(0f, 0.05f, depthFactor);
                        if (rng.NextDouble() > fossilChance)
                        {
                            continue;
                        }

                        int clusterSize = rng.Next(1, 4);
                        for (int i = 0; i < clusterSize; i++)
                        {
                            Vector3Int offset = new Vector3Int(
                                rng.Next(-1, 2),
                                rng.Next(-1, 2),
                                0
                            );
                            
                            Vector3Int spawnPos = pos + offset;
                            TileBase spawnTile = floorsTilemap.GetTile(spawnPos);

                            
                            if (spawnTile != dungeonSettings.VolcanicRubbleTile)
                            {
                                continue;
                            }
                            if (collisionDecorationTilemap.HasTile(spawnPos) || nonCollisionDecorationTilemap.HasTile(spawnPos) || vegetationTilemap.HasTile(spawnPos))
                            {
                                continue;
                            }
                            if (IsTrap(spawnPos))
                            {
                                continue;
                            }
                            if (IsNearDoor(spawnPos) || IsNearLadder(spawnPos))
                            {
                                continue;
                            }
                            
                            collisionDecorationTilemap.SetTile(spawnPos, dungeonSettings.FossilTile);
                        }
                    }
                }
            }
        }

        private bool IsNearLava(Vector3Int pos)
        {
            return floorsTilemap.GetTile(pos + Vector3Int.up) == dungeonSettings.LavaTile ||
                   floorsTilemap.GetTile(pos + Vector3Int.down) == dungeonSettings.LavaTile ||
                   floorsTilemap.GetTile(pos + Vector3Int.left) == dungeonSettings.LavaTile ||
                   floorsTilemap.GetTile(pos + Vector3Int.right) == dungeonSettings.LavaTile;
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
                    if (wallsTilemap.HasTile(pos) || floorsTilemap.HasTile(pos))
                    {
                        continue;
                    }
                    
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), dungeonSettings.BackgroundTile);
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
            ladderTilemap.ClearAllTiles();
            collisionDecorationTilemap.ClearAllTiles();
            nonCollisionDecorationTilemap.ClearAllTiles();
            backgroundTilemap.ClearAllTiles();
            vegetationTilemap.ClearAllTiles();
            
            rooms.Clear(); 
            corridorFloorPositions.Clear();
            trapPositions.Clear();
            spikesTraps.Clear();
            corridorArrowTraps.Clear();
            flameJetsTraps.Clear();
            
            //destroy the old spawned gameobjects
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                else
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}