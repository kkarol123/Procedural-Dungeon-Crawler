using UnityEngine;
using System.Collections.Generic;
using PlayerScripts;

namespace GameManagerScripts
{
    public class RoomCombatManager : MonoBehaviour
    {
        private Dictionary<int, int> enemiesRemainingPerRoom = new Dictionary<int, int>();
        private HashSet<int> clearedRooms = new HashSet<int>();
        [SerializeField] private GameObject key;

        public void InitialiseEnemy(int roomIndex)
        {
            if (!enemiesRemainingPerRoom.ContainsKey(roomIndex))
            {
                enemiesRemainingPerRoom.Add(roomIndex, 0);
            }
            
            enemiesRemainingPerRoom[roomIndex]++;
        }

        public void NotifyEnemyDied(int roomIndex, Vector3 deathPosition)
        {
            enemiesRemainingPerRoom[roomIndex]--;

            if (enemiesRemainingPerRoom[roomIndex] <= 0 && !clearedRooms.Contains(roomIndex))
            {
                clearedRooms.Add(roomIndex);

                Vector3 keySpawnPosition = deathPosition;
                keySpawnPosition.z = -4f;

                Instantiate(key, keySpawnPosition, Quaternion.identity);
            }
        }
    }
}