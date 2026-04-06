using UnityEngine;
using UnityEngine.Tilemaps;

namespace CorridorTrapScripts.ArrowTrap
{
    public class ArrowTrapManager : MonoBehaviour
    {
        public static ArrowTrapManager Instance { get; private set; }

        [SerializeField] private Tilemap wallsTilemap;
        [SerializeField] private GameObject corridorArrowPrefab;

        private void Awake()
        {
            Instance = this;
        }

        public void FireArrow(Vector3Int launcherPosition, Vector2Int fireDirection)
        {
            Vector3 worldPosition = wallsTilemap.GetCellCenterWorld(launcherPosition);
            worldPosition += new Vector3(fireDirection.x, fireDirection.y, 0f) * 1.1f;   //1.1f to not spawn in the wall and collide with the same wall which shot it lol
            worldPosition.z = -4f;
            
            GameObject arrowObject = Instantiate(corridorArrowPrefab, worldPosition, Quaternion.identity);
            CorridorArrow corridorArrow = arrowObject.GetComponent<CorridorArrow>();
            corridorArrow.Initialise(fireDirection);
        }
    }
}