using UnityEngine;
using System.Collections.Generic;
using EnemyScripts;

namespace PlayerScripts{
    public class PlayerTargetDetector : MonoBehaviour
    {
        private readonly List<EnemyScript> enemiesInRange = new List<EnemyScript>();
        public List<EnemyScript> EnemiesInRange => enemiesInRange;

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyScript>();
            }
            if (enemy == null)
            {
                return;
            }
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();

            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyScript>();
            }
            if (enemy == null)
            {
                return;
            }

            enemiesInRange.Remove(enemy);
        }

        public void CleanList()
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                if (enemiesInRange[i] == null)
                {
                    enemiesInRange.RemoveAt(i);
                }
            }
        }
    }
}