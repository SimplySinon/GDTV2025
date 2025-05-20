using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]

    [Header("Testing Data")]
    [SerializeField] bool useTestingConfigs;
    [SerializeField] Transform testingPlayer;
    [SerializeField] Collider2D testingBounds;
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
            SpawnEnemies(testingSpawnConfigs, testingPlayer, testingBounds);
        }
    }

    public void SpawnEnemies(List<EnemySpawnConfig> enemySpawnConfigs, Transform player, Collider2D bounds)
    {
        foreach (EnemySpawnConfig config in enemySpawnConfigs)
        {
            foreach (Vector2 location in config.SpawnLocations)
            {
                EnemyController instance = Instantiate(config.Enemy, enemyHolder);
                instance.Initialize(config.Config, location, player, bounds);
                enemies.Add(instance);
            }
        }
    }
}