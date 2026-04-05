using UnityEngine;

namespace TrapScripts
{
    [System.Serializable]
    public class CorridorArrowDataScript
    {
        public Vector3Int platePosition;
        public Vector3Int launcherPosition;
        public Vector2Int fireDirection;

        public CorridorArrowDataScript(Vector3Int platePos, Vector3Int launcherPos, Vector2Int direction)
        {
            platePosition = platePos;
            launcherPosition = launcherPos;
            fireDirection = direction;
        }
    }
}