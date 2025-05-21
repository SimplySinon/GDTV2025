using UnityEngine;

[System.Serializable]
public class PlayerAttack
{
    public int EnemyId;
    public float AttackDamage;

    public PlayerAttack(int id, float dmg)
    {
        EnemyId = id;
        AttackDamage = dmg;
    }
}