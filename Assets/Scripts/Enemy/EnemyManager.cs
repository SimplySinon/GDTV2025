using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]

    [Header("Testing Data")]
    [SerializeField] bool useTestingConfigs;
    [SerializeField] List<EnemySpawnConfig> testingSpawnConfigs;

    List<EnemyController> enemies;
    Transform enemyHolder;

    void Start()
    {
        enemyHolder = new GameObject("EnemyHolder").transform;
        enemyHolder.transform.parent = null;

        enemies = new();

        if (testingSpawnConfigs != null)
        {
            SpawnEnemies(testingSpawnConfigs);
        }
    }

    public void SpawnEnemies(List<EnemySpawnConfig> enemySpawnConfigs)
    {
        foreach (EnemySpawnConfig config in enemySpawnConfigs)
        {
            foreach (Vector2 location in config.SpawnLocations)
            {
                EnemyController instance = Instantiate(config.Enemy, enemyHolder);
                instance.Initialize(config.Config, location);
                enemies.Add(instance);
            }
        }
    }
}