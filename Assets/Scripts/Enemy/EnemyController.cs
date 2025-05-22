using System;
using System.Collections;
using Unity.Mathematics;
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
    [SerializeField] float delayBeforeDestroy = 0.75f;
    [SerializeField] float changeDirectionThreshold = 0.01f;
    [SerializeField] float meleeAttackRange = 1f;

    [Header("Melee Collider Settings")]
    [SerializeField] private BoxCollider2D meleeAttackCollider;
    [SerializeField] private Vector2 downAttackPosition;
    [SerializeField] private Vector2 upAttackPosition;
    [SerializeField] private Vector2 leftAttackPosition;
    [SerializeField] private Vector2 rightAttackPosition;

    [Header("Knockback Settings")]
    [SerializeField] bool hasKnockback;
    [SerializeField] float forceMagnitude;
    [SerializeField] float knockbackDuration;
    [SerializeField] float delayBeforeKnockback;

    private Rigidbody2D rb;
    private Enemy self;
    private EnemyMovementAIEngine movementEngine;
    private EnemyAnimator enemyAnimator;

    Vector2 previousPosition;
    State currentState;

    float attackTimer = 0;
    float attackDuration;

    float nextRangedProjectile;

    public void Initialize(EnemyConfigSO type, Vector2 position, Transform player, Collider2D bounds)
    {
        self = new(GetInstanceID(), type, player, bounds);
        transform.position = position;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        enemyAnimator = GetComponent<EnemyAnimator>();

        meleeAttackCollider.includeLayers = playerMask;
        nextRangedProjectile = Time.time;

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
            ApplyKnockBack();
        }
    }

    IEnumerator DestroyEnemy()
    {
        // Hacky way to delay Destroy to allow for animations and effects to complete
        yield return new WaitForSeconds(delayBeforeDestroy);
        Helpers.RaiseIfNotNull(enemyDestroyedEventChannel, self.EnemyId);
    }

    public int GetEnemyId()
    {
        return self.EnemyId;
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        movementEngine.Update();

        float distanceFromPreviousPos = Vector2.Distance(previousPosition, transform.position);
        if (distanceFromPreviousPos > changeDirectionThreshold && !movementEngine.IsApplyingKnockBack())
        {
            Vector2 dir = (Vector2)transform.position - previousPosition;
            RepositionAttackCollider(dir);
            enemyAnimator.ChangeEnemyMoveDirection(dir);
            previousPosition = (Vector2)transform.position;
        }

        // Attack Related Logic
        AttackPlayer();
    }

    void AttackPlayer()
    {
        attackDuration -= Time.deltaTime;
        if (currentState != State.Moving && attackDuration <= 0)
        {
            enemyAnimator.ChangeEnemyState(State.Moving);
            currentState = State.Moving;
            meleeAttackCollider.enabled = false;
            movementEngine.SetIsAttacking(false);
            attackTimer = self.EnemyConfig.AttackCooldown;
            nextRangedProjectile = Time.time;

        }

        attackTimer -= Time.deltaTime;
        if (attackTimer > 0) return;

        float distanceFromPlayer = Vector2.Distance(transform.position, self.Player.position);

        if (self.EnemyConfig.IsRanged && distanceFromPlayer <= self.EnemyConfig.AttackRange)
        {
            AttackPlayerFromRange();
            if (currentState != State.Attacking)
            {
                attackDuration = self.EnemyConfig.AttackDuration;
                movementEngine.SetIsAttacking(true);
                currentState = State.Attacking;
                enemyAnimator.ChangeEnemyState(State.Attacking);
            }
        }
        else if (!self.EnemyConfig.IsRanged && distanceFromPlayer <= meleeAttackRange)
        {
            if (currentState != State.Attacking)
            {
                attackDuration = self.EnemyConfig.AttackDuration;
                movementEngine.SetIsAttacking(true);
                currentState = State.Attacking;
                enemyAnimator.ChangeEnemyState(State.Attacking);
            }
            meleeAttackCollider.enabled = true;
        }
    }

    private void RepositionAttackCollider(Vector2 moveDirection)
    {
        float x = moveDirection.x;
        float y = moveDirection.y;

        if (y < 0)
        {
            meleeAttackCollider.transform.localPosition = downAttackPosition;
        }
        else if (y > 0)
        {
            meleeAttackCollider.transform.localPosition = upAttackPosition;
        }
        else if (y == 0 && x < 0)
        {
            meleeAttackCollider.transform.localPosition = leftAttackPosition;
        }
        else if (y == 0 && x > 0)
        {
            meleeAttackCollider.transform.localPosition = rightAttackPosition;
        }
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

    void AttackPlayerFromRange()
    {
        if (nextRangedProjectile <= Time.time)
        {
            Debug.Log("Spawning Projectile");
            Projectile projectile = Instantiate(self.EnemyConfig.Projectile, meleeAttackCollider.transform.position, quaternion.identity);
            projectile.transform.parent = null;
            float speed = self.EnemyConfig.ProjectileSpeed;
            Vector2 dir = self.Player.position - transform.position;
            enemyAnimator.ChangeEnemyMoveDirection(dir);
            RepositionAttackCollider(dir.normalized);
            projectile.Initialize(speed, dir, playerMask, self.MovementBounds, DamagePlayer);
            nextRangedProjectile = Time.time + self.EnemyConfig.ProjectileFireRate;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // We can consider this an attack for now, attack types will be added later
        if (playerMask.Contains(collision.gameObject.layer) && !self.EnemyConfig.IsRanged)
        {
            self.TakeDamage(1);
            DamagePlayer();
            ApplyKnockBack();
        }
    }

    void ApplyKnockBack()
    {
        if (!movementEngine.IsApplyingKnockBack())
        {
            StartCoroutine(movementEngine.StopKnockbackAfterDelay(CheckIfIAmDead));
        }
    }

    void CheckIfIAmDead()
    {
        if (self.IsDead())
        {
            // Add any death effects for the Enemy, the following line will destroy the enemy object
            enemyAnimator.ChangeEnemyState(State.Dead);
            StartCoroutine(DestroyEnemy());
        }
    }

    void DamagePlayer()
    {
        Helpers.RaiseIfNotNull(enemyAttackEventChannel, self.GetAttackDamage());
    }
}