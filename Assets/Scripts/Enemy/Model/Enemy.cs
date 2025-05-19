using System;
using UnityEngine;

public class Enemy
{
    public int EnemyId;
    public EnemyTypeConfigSO EnemyConfig;

    public Enemy(int id, EnemyTypeConfigSO config)
    {
        EnemyId = id;
        EnemyConfig = config;
    }
}