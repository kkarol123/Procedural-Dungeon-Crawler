using UnityEngine;

namespace CorridorTrapScripts.SpikesTrap
{
    [System.Serializable]
    public class SpikesData
    {
        public Vector3Int spikePosition;

        public SpikesData(Vector3Int position)
        {
            spikePosition = position;
        }
    }
}