using UnityEngine;

public class Enemy
{
    public int EnemyId;
    public EnemyConfigSO EnemyConfig;
    public Transform Player;
    public Collider2D MovementBounds;

    // Instance Values Backed up from SO
    private float maxHealth;

    public Enemy(int id, EnemyConfigSO config, Transform target, Collider2D bounds)
    {
        EnemyId = id;
        EnemyConfig = config;
        Player = target;
        MovementBounds = bounds;
        maxHealth = config.MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        maxHealth -= damage;
        maxHealth = Mathf.Clamp(maxHealth, 0, EnemyConfig.MaxHealth);
    }

    public bool IsDead()
    {
        if (maxHealth == 0) return true;

        return false;
    }

    public float GetAttackDamage()
    {
        return Random.Range(EnemyConfig.MinDamage, EnemyConfig.MaxDamage);
    }
}