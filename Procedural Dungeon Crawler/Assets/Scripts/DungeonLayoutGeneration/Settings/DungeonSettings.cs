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
        
        
        
        
    
        //Randomised Constraints
        [FoldoutGroup("Randomised Constraints")] 
        [SerializeField]
        private int minRooms = 3;
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
        private TileBase floorTile;
        public TileBase FloorTile => floorTile;

        [FoldoutGroup("Tiles")]
        [SerializeField]
        private TileBase wallTile;
        public TileBase WallTile => wallTile;


        [FoldoutGroup("Tiles")] 
        [SerializeField]
        private TileBase doorTile;
        public TileBase DoorTile => doorTile;
    }
}