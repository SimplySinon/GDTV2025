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
    public bool roomEnemiesDefeated = false;

    private bool waveSpawned = false;

    public Transform enemiesContainer;
    public AudioSource ambientMusic;
    public AudioSource battleMusic;

    private bool playerInside = false;
    private bool battleMusicPlaying = false;


    void Start()
    {
        config = new();
        config.RoomId = GetInstanceID();
        config.RoomBounds = GetComponent<Collider2D>();

        if (config.RoomBounds == null)
        {
            throw new UnityException("Room Bounds Collider not found on room object");
        }
        config.RoomBounds.isTrigger = true;

        currentWave = -1;
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(onWaveCompletedEventChannel, OnWaveCompleted);
    }

    void Update()
    {
        
        
            CheckIfEnemiesDefeated();
        
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
            
            OnRoomTriggerEnter();
            SpawnNextWave();
            InvokeRepeating("CheckEnemies", 1f, 1f);
        }
    }

    public void OnRoomTriggerEnter()
    {
        if (useRoomPosition)
            cameraConfig.CameraPosition = transform.position;

        Helpers.RaiseIfNotNull(cameraConfigEventChannel, cameraConfig);
        InvokeRepeating("CheckEnemies", 1f, 1f);
    }

    private void SpawnNextWave()
    {
        currentWave += 1;
        Helpers.RaiseIfNotNull(roomConfigEventChannel, config);
        List<EnemySpawnConfig> nextWave = enemySpawnConfigs.FindAll(e => e.WaveNo == currentWave);

        if (nextWave.Count > 0)
        {
            waveSpawned = true;
            roomEnemiesDefeated = false;
            Helpers.RaiseIfNotNull(spawnEnemiesEventChannel, nextWave);
        }
        else
        {
            roomEnemiesDefeated = true;
        }
    }

    private void CheckIfEnemiesDefeated()
    {
        if (!waveSpawned || roomEnemiesDefeated) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int enemiesInRoom = 0;
        foreach (GameObject enemy in enemies)
        {
            if (config.RoomBounds.bounds.Contains(enemy.transform.position))
                enemiesInRoom++;
        }

        if (enemiesInRoom == 0)
        {
            roomEnemiesDefeated = true;
            waveSpawned = false;
            Debug.Log("All enemies defeated in room: " + config.RoomId);
         
        }
    }

    void CheckEnemies()
    {
        int aliveCount = 0;

        foreach (Transform enemy in enemiesContainer)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                aliveCount++;
            }
        }

        if (aliveCount > 0 && !battleMusicPlaying)
        {
            ambientMusic.Stop();
            battleMusic.Play();
            battleMusicPlaying = true;
        }
        else if (aliveCount == 0 && battleMusicPlaying)
        {
            battleMusic.Stop();
            ambientMusic.Play();
            battleMusicPlaying = false;
        }
    }
}
