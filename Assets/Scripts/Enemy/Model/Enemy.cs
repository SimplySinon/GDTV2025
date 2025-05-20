using System;
using UnityEngine;

public class Enemy
{
    public int EnemyId;
    public EnemyConfigSO EnemyConfig;
    public Transform Player;
    public Collider2D MovementBounds;

    public Enemy(int id, EnemyConfigSO config, Transform target, Collider2D bounds)
    {
        EnemyId = id;
        EnemyConfig = config;
        Player = target;
        MovementBounds = bounds;
    }
}