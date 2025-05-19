using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnConfig
{
    public EnemyController Enemy;
    public EnemyTypeConfigSO Config;
    public List<Vector2> SpawnLocations;
}