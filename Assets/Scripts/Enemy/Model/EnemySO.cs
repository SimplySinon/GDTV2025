using UnityEngine;

[CreateAssetMenu(menuName = "GDTV2025/Enemy/EnemyTypeConfigSO", fileName = "EnemyTypeConfig")]
public class EnemyTypeConfigSO : ScriptableObject
{
    public float MoveSpeed;
    public float PlayerSearchCooldown;
    public float AttackCooldown;
    public float MaxLife;
    public float MaxDamage;
    public float MinDamage;
}