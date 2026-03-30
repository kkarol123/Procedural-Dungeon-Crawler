using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

namespace DungeonLayoutGeneration.Settings
{
    [System.Serializable]
    public class DungeonSettings
    {
        //Fixed Constraints
        [FoldoutGroup("Fixed Constraints")] 
        [SerializeField]
        private int seed = 12345;
        public int Seed => seed;
        
        [FoldoutGroup("Fixed Constraints")] 
        [SerializeField]
        private float noiseScale = 0.1f;
        public float NoiseScale => noiseScale;
        
        
        
        
    
        //Randomised Constraints
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minRooms = 2;
        public int MinRooms => minRooms;

        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int maxRooms = 7;
        public int MaxRooms => maxRooms;

        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minWidth = 12;
        public int MinWidth => minWidth;

        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int maxWidth = 25;
        public int MaxWidth => maxWidth;

        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minHeight = 8;
        public int MinHeight => minHeight;

        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int maxHeight = 16;
        public int MaxHeight => maxHeight;

        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("The minimum distance between rooms")]
        [SerializeField]
        private int minDistance = 8;
        public int MinDistance => minDistance;

        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("The maximum distance between rooms")]
        [SerializeField]
        private int maxDistance = 15;
        public int MaxDistance => maxDistance;

    
        
        
        //Tiles
        [FoldoutGroup("Tiles")]
        [SerializeField]
        private TileBase wallTile;
        public TileBase WallTile => wallTile;
        
        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase doorTile;
        public TileBase DoorTile => doorTile;

        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase exitDoorTile;
        public TileBase ExitDoorTile => exitDoorTile;

        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase backgroundTile;
        public TileBase BackgroundTile => backgroundTile;
        
        
        
        
        [FoldoutGroup("Tiles/All Biome Tiles")] 
        [SerializeField]
        private TileBase dryStoneTile;
        public TileBase DryStoneTile => dryStoneTile;
        
        [FoldoutGroup("Tiles/All Biome Tiles")] 
        [SerializeField]
        private TileBase crackedStoneTile;
        public TileBase CrackedStoneTile => crackedStoneTile;
        
        [FoldoutGroup("Tiles/All Biome Tiles")] 
        [SerializeField]
        private TileBase mossyStoneTile;
        public TileBase MossyStoneTile => mossyStoneTile;
        
        
        
        
        [FoldoutGroup("Tiles/Flooded Biome Tiles")] 
        [SerializeField]
        private TileBase dampStoneTile;
        public TileBase DampStoneTile => dampStoneTile;
        
        [FoldoutGroup("Tiles/Flooded Biome Tiles")] 
        [SerializeField]
        private TileBase puddleTile;
        public TileBase PuddleTile => puddleTile;
        
        
        
        
        [FoldoutGroup("Tiles/Volcanic Biome Tiles")] 
        [SerializeField]
        private TileBase volanicRubbleTile;
        public TileBase VolanicRubbleTile => volanicRubbleTile;
        
        [FoldoutGroup("Tiles/Volcanic Biome Tiles")] 
        [SerializeField]
        private TileBase lavaTile;
        public TileBase LavaTile => lavaTile;
        
        
        
        
        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase skullTile;
        public TileBase SkullTile => skullTile;

        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase bloodTile;
        public TileBase BloodTile => bloodTile;
        
        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase holeTile;
        public TileBase HoleTile => holeTile;
        
        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase cobwebTile;
        public TileBase CobwebTile => cobwebTile;
        
        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase candlesTile;
        public TileBase CandlesTile => candlesTile;
        
        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase bookshelfTile;
        public TileBase BookshelfTile => bookshelfTile;
        
        [FoldoutGroup("Tiles/Decorations")]
        [SerializeField]
        private TileBase tableTile;
        public TileBase TableTile => tableTile;
        
        
        
        [FoldoutGroup("Tiles/Secret Room")]
        [SerializeField]
        private TileBase closedChestTile;
        public TileBase ClosedChestTile => closedChestTile;
        
        [FoldoutGroup("Tiles/Secret Room")]
        [SerializeField]
        private TileBase openChestTile;
        public TileBase OpenChestTile => openChestTile;
        
        [FoldoutGroup("Tiles/Secret Room")]
        [SerializeField]
        private TileBase secretWallTile;
        public TileBase SecretWallTile => secretWallTile;
        
        [FoldoutGroup("Tiles/Secret Room")]
        [SerializeField]
        private TileBase ladderTile;
        public TileBase LadderTile => ladderTile;
    }
}