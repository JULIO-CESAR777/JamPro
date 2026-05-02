using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;

        [Header("Patrulla (opcional)")]
        public Transform pointA;
        public Transform pointB;
    }

    public List<SpawnData> enemies = new List<SpawnData>();

    private int aliveEnemies = 0;
    private CombatZone combatZone;

    public void SetCombatZone(CombatZone zone)
    {
        combatZone = zone;
    }

    public void SpawnAll()
    {
        aliveEnemies = 0;

        foreach (SpawnData data in enemies)
        {
            if (data.enemyPrefab == null || data.spawnPoint == null)
                continue;

            GameObject enemyObj = Instantiate(
                data.enemyPrefab,
                data.spawnPoint.position,
                Quaternion.identity
            );

            aliveEnemies++;

            EnemyDeath death = enemyObj.GetComponent<EnemyDeath>();
            if (death != null)
            {
                death.SetSpawner(this);
            }

            EnemyPatrol patrol = enemyObj.GetComponent<EnemyPatrol>();

            if (patrol != null && data.pointA != null && data.pointB != null)
            {
                patrol.SetPatrolPoints(data.pointA, data.pointB);
            }
        }

        if (aliveEnemies == 0)
        {
            if (combatZone != null)
                combatZone.ZoneCleared();
        }
    }

    public void EnemyDied()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            if (combatZone != null)
                combatZone.ZoneCleared();
        }
    }
}