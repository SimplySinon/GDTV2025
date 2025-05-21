using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : RegulatorSingleton<EnemyManager>
{
    [Header("Inbound Communication")]
    [SerializeField] EnemySpawnConfigListEventChannelSO spawnEnemiesEventChannel;
    [SerializeField] RoomConfigEventChannelSO currentRoomEventChannel;
    [SerializeField] IntEventChannelSO enemyDestroyedEventChannel;
    [SerializeField] TransformEventChannelSO playerTransformEventChannel;

    [Header("Outbound Communication")]
    [SerializeField] IntEventChannelSO waveCompletedEventChannel;

    [Header("Testing Data")]
    [SerializeField] bool useTestingConfigs;
    [SerializeField] Transform testingPlayer;
    [SerializeField] Collider2D testingBounds;
    [SerializeField] List<EnemySpawnConfig> testingSpawnConfigs;

    List<EnemyController> enemies;
    Transform enemyHolder;
    RoomConfig currentRoom;
    Transform player;

    void Start()
    {
        enemyHolder = new GameObject("EnemyHolder").transform;
        enemyHolder.transform.parent = null;

        enemies = new();

        if (useTestingConfigs && testingSpawnConfigs != null)
        {
            SpawnEnemies(testingSpawnConfigs, testingPlayer, testingBounds);
        }
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(enemyDestroyedEventChannel, OnEnemyDestroyed);
        Helpers.SubscribeIfNotNull(currentRoomEventChannel, OnCurrentRoom);
        Helpers.SubscribeIfNotNull(spawnEnemiesEventChannel, OnSpawnEnemiesList);
        Helpers.SubscribeIfNotNull(playerTransformEventChannel, OnPlayerTransform);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(enemyDestroyedEventChannel, OnEnemyDestroyed);
        Helpers.UnsubscribeIfNotNull(currentRoomEventChannel, OnCurrentRoom);
        Helpers.UnsubscribeIfNotNull(spawnEnemiesEventChannel, OnSpawnEnemiesList);
    }

    void Update()
    {

    }

    void OnPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
    }

    void OnSpawnEnemiesList(List<EnemySpawnConfig> spawnConfigs)
    {
        if (spawnConfigs.Count > 0 && currentRoom != null)
        {
            Debug.Log($"RoomID: {currentRoom.RoomId}");
            SpawnEnemies(spawnConfigs, player, currentRoom.RoomBounds);
        }
    }

    void OnEnemyDestroyed(int enemyId)
    {
        EnemyController controller = enemies.Find(e => e.GetEnemyId() == enemyId);

        if (controller != null)
        {
            enemies.Remove(controller);
            Destroy(controller.gameObject);
        }

        if (enemies.Count == 0)
        {
            Helpers.RaiseIfNotNull(waveCompletedEventChannel, currentRoom.RoomId);
        }
    }

    void OnCurrentRoom(RoomConfig roomConfig)
    {
        // if (currentRoom != null)
        // {
        //     // Cleanup Current Room First if Anything Survived;
        //     for (int i = enemies.Count - 1; i > 0; i--)
        //     {
        //         Destroy(enemies[i].gameObject);
        //     }
        // }

        currentRoom = roomConfig;
    }

    void SpawnEnemies(List<EnemySpawnConfig> enemySpawnConfigs, Transform player, Collider2D bounds)
    {
        foreach (EnemySpawnConfig config in enemySpawnConfigs)
        {
            foreach (Vector2 location in config.SpawnLocations)
            {
                EnemyController instance = Instantiate(config.Enemy, enemyHolder);
                instance.Initialize(config.Config, (Vector2)bounds.transform.position + location, player, bounds);
                enemies.Add(instance);
            }
        }
    }
}