using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    [Header("Inbound Communication")]
    [SerializeField] IntEventChannelSO onWaveCompletedEventChannel;

    [Header("Outbound Communication")]
    [SerializeField] CameraConfigEventChannelSO cameraConfigEventChannel;
    [SerializeField] RoomConfigEventChannelSO roomConfigEventChannel;
    [SerializeField] EnemySpawnConfigListEventChannelSO spawnEnemiesEventChannel;

    [Header("Room Camera Config")]
    [SerializeField] CameraConfig cameraConfig;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool useRoomPosition;

    [Header("Room Enemy Config")]
    [SerializeField] List<EnemySpawnConfig> enemySpawnConfigs;
    private int currentWave;
    private RoomConfig config;
    private bool roomEnemiesDefeated;

    void Start()
    {
        config = new();
        config.RoomId = GetInstanceID();
        config.RoomBounds = GetComponent<Collider2D>();

        if (config.RoomBounds == null)
        {
            throw new UnityException("Room Bounds Collider not found on room object");
        }

        currentWave = -1;
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(onWaveCompletedEventChannel, OnWaveCompleted);
    }

    void OnWaveCompleted(int id)
    {
        if (config.RoomId == id)
        {
            SpawnNextWave();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (playerLayer.Contains(other.gameObject.layer))
        {
            Debug.Log("Player Has entered");
            OnRoomTriggerEnter();
            SpawnNextWave();
        }
    }

    public void OnRoomTriggerEnter()
    {
        if (useRoomPosition)
            cameraConfig.CameraPosition = transform.position;

        Helpers.RaiseIfNotNull(cameraConfigEventChannel, cameraConfig);
    }

    private void SpawnNextWave()
    {
        currentWave += 1;
        List<EnemySpawnConfig> nextWave = enemySpawnConfigs.FindAll(e => e.WaveNo == currentWave);

        Debug.Log($"Count: {nextWave.Count}");
        if (nextWave.Count > 0)
        {
            Helpers.RaiseIfNotNull(roomConfigEventChannel, config);
            Helpers.RaiseIfNotNull(spawnEnemiesEventChannel, nextWave);
        }
        else
        {
            roomEnemiesDefeated = true;
        }
    }
}