using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Inbound Communication")]
    [SerializeField] PlayerAttackEventChannelSO playerAttackEventChannel;

    [Header("Outbound Communication")]
    [SerializeField] IntEventChannelSO enemyDestroyedEventChannel;
    [SerializeField] FloatEventChannelSO enemyAttackEventChannel;

    [Header("Enemy Settings")]
    [SerializeField] LayerMask playerMask;

    [Header("Knockback Settings")]
    [SerializeField] bool hasKnockback;
    [SerializeField] float forceMagnitude;
    [SerializeField] float knockbackDuration;
    [SerializeField] float delayBeforeKnockback;

    private Rigidbody2D rb;
    private Enemy self;
    private EnemyMovementAIEngine movementEngine;

    public void Initialize(EnemyConfigSO type, Vector2 position, Transform player, Collider2D bounds)
    {
        self = new(GetInstanceID(), type, player, bounds);
        transform.position = position;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        if (rb == null) throw new UnityException("Rigidbody2D Componet not found on Enemy Object");

        InitializeMovementEngine();
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(playerAttackEventChannel, OnPlayerAttack);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(playerAttackEventChannel, OnPlayerAttack);
    }

    void OnPlayerAttack(PlayerAttack attack)
    {
        if (attack.EnemyId == GetEnemyId())
        {
            self.TakeDamage(attack.AttackDamage);
            if (!movementEngine.IsApplyingKnockBack())
                StartCoroutine(movementEngine.StopKnockbackAfterDelay(() =>
                {
                    if (self.IsDead())
                    {
                        // Add any death effects for the Enemy, the following line will destroy the enemy object
                        Helpers.RaiseIfNotNull(enemyDestroyedEventChannel, self.EnemyId);
                    }
                }));
        }
    }

    public int GetEnemyId()
    {
        return self.EnemyId;
    }

    void Update()
    {
        movementEngine.Update();
    }

    void InitializeMovementEngine()
    {
        switch (self.EnemyConfig.MovementAIType)
        {
            case EnemyMovementAIType.Charge:
                movementEngine = new ChargeAI(rb, gameObject, self.Player, self.MovementBounds, self.EnemyConfig);
                break;
            case EnemyMovementAIType.FollowWithBuffer:
                movementEngine = new FollowWithBufferAI(rb, gameObject, self.Player, self.MovementBounds, self.EnemyConfig);
                break;
            case EnemyMovementAIType.KeepRelativeOffset:
                movementEngine = new KeepRelativeOffsetAI(rb, gameObject, self.Player, self.MovementBounds, self.EnemyConfig);
                break;
            case EnemyMovementAIType.RandomPatrol:
                movementEngine = new RandomPatrolAI(rb, gameObject, self.Player, self.MovementBounds, self.EnemyConfig);
                break;
            case EnemyMovementAIType.WanderAndCharge:
                movementEngine = new WanderAndChargeAI(rb, gameObject, self.Player, self.MovementBounds, self.EnemyConfig);
                break;
        }

        if (hasKnockback)
            movementEngine.InitializeKnockBack(forceMagnitude, knockbackDuration, delayBeforeKnockback);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // We can consider this an attack for now, attack types will be added later
        if (playerMask.Contains(collision.gameObject.layer))
        {
            Helpers.RaiseIfNotNull(enemyAttackEventChannel, self.GetAttackDamage());
        }
    }
}