using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Inbound Communication")]
    [SerializeField] IntEventChannelSO enemyIdEventChannel;

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
        Helpers.SubscribeIfNotNull(enemyIdEventChannel, OnEnemyTakingDamage);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(enemyIdEventChannel, OnEnemyTakingDamage);
    }

    void OnEnemyTakingDamage(int enemyId)
    {
        if (enemyId == GetEnemyId())
        {
            Debug.Log($"I {name}:{GetEnemyId()} am taking Damage");
            if (!movementEngine.IsApplyingKnockBack())
                StartCoroutine(movementEngine.StopKnockbackAfterDelay());
        }
    }

    // private System.Collections.IEnumerator StopKnockbackAfterDelay(float duration, Vector3 impactPoint)
    // {
    //     applyingKnockBack = true;
    //     yield return new WaitForSeconds(delayBeforeKnockback);
    //     // Calculate the knockback direction (from target to impact point)
    //     Vector3 knockbackDirection = (transform.position - impactPoint).normalized;

    //     // Apply the force
    //     if (rb != null)
    //     {
    //         rb.AddForce(knockbackDirection * forceMagnitude, ForceMode2D.Impulse);
    //     }

    //     yield return new WaitForSeconds(duration);
    //     rb.linearVelocity = Vector3.zero; // Stop the target's movement
    //     rb.angularVelocity = 0; // Stop the target's rotation
    //     applyingKnockBack = false;
    // }
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

}