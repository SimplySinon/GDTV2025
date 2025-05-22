using UnityEngine;

[System.Serializable]
public class PlayerAttack
{
    public int EnemyId;
    public float AttackDamage;
    public bool IsRanged;
    public PlayerAttack(int id, float dmg, bool isRanged = false)
    {
        EnemyId = id;
        AttackDamage = dmg;
        IsRanged = isRanged;
    }
}