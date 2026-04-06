using CorridorTrapScripts.SpikesTrap;
using UnityEngine;

namespace CorridorTrapScripts.FlameJetsTrap
{
    [System.Serializable]
    public class FlameJetData
    {
        public Vector3Int flamePosition;
        public Vector2Int flameDirection;

        public FlameJetData(Vector3Int position, Vector2Int direction)
        {
            flamePosition = position;
            flameDirection = direction;
        }
    }
}
