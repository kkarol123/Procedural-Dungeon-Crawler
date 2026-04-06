using UnityEngine;

namespace CorridorTrapScripts.ArrowTrap
{
    [System.Serializable]
    public class CorridorArrowData
    {
        public Vector3Int platePosition;
        public Vector3Int launcherPosition;
        public Vector2Int fireDirection;

        public CorridorArrowData(Vector3Int platePos, Vector3Int launcherPos, Vector2Int direction)
        {
            platePosition = platePos;
            launcherPosition = launcherPos;
            fireDirection = direction;
        }
    }
}