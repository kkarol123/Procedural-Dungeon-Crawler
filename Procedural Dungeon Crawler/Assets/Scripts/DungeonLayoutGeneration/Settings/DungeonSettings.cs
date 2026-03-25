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
    
    
        //Randomised Constraints
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minRooms = 3;
    
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int maxRooms = 7;
        
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minWidth = 12;
    
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int maxWidth = 25;
        
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minHeight = 8;
    
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int maxHeight = 16;
    
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("The minimum distance between rooms")]
        [SerializeField]
        private int minDistance = 8;
    
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("The maximum distance between rooms")]
        [SerializeField]
        private int maxDistance = 15;
    
    
        //Tiles
        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase floorTile;
        
        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase topWallTile;
        
        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase sideTile;
        
        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase bottomWallTile;
        
        
        
        
        
        
        public int Seed => seed;
        public int MinRooms => minRooms;
        public int MaxRooms => maxRooms;
        public int MinWidth => minWidth;
        public int MaxWidth => maxWidth;
        public int MinHeight => minHeight;
        public int MaxHeight => maxHeight;
        public int MinDistance => minDistance;
        public int MaxDistance => maxDistance;
        public TileBase FloorTile => floorTile;
        public TileBase TopWallTile => topWallTile;
        public TileBase SideWallTile => sideTile;
        public TileBase BottomWallTile => bottomWallTile;
    }
}