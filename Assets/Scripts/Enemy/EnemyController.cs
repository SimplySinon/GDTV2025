using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Inbound Communication")]
    [SerializeField] Vector2EventChannelSO playerPositionEventChannel;
    [SerializeField] IntEventChannelSO enemyIdEventChannel;

    [Header("Knockback Settings")]
    [SerializeField] bool hasKnockback;
    [SerializeField] float forceMagnitude;
    [SerializeField] float knockbackDuration;
    [SerializeField] float delayBeforeKnockback;

    private Rigidbody2D rb;
    private Enemy self;
    private Vector2 playerPosition;
    private bool applyingKnockBack;

    public void Initialize(EnemyTypeConfigSO type, Vector2 position)
    {
        self = new(GetInstanceID(), type);
        transform.position = position;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(playerPositionEventChannel, OnPlayerPosition);
        Helpers.SubscribeIfNotNull(enemyIdEventChannel, OnEnemyTakingDamage);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(playerPositionEventChannel, OnPlayerPosition);
        Helpers.UnsubscribeIfNotNull(enemyIdEventChannel, OnEnemyTakingDamage);
    }

    void OnPlayerPosition(Vector2 pos)
    {
        playerPosition = pos;
    }

    void OnEnemyTakingDamage(int enemyId)
    {
        if (enemyId == GetEnemyId())
        {
            Debug.Log($"I {name}:{GetEnemyId()} am taking Damage");

            if (!applyingKnockBack)
                StartCoroutine(StopKnockbackAfterDelay(knockbackDuration, playerPosition));
        }
    }

    public int GetEnemyId()
    {
        return self.EnemyId;
    }

    private void Move()
    {
        // Do Move
    }

    private void Attack()
    {
        // Do Attak
    }


    private System.Collections.IEnumerator StopKnockbackAfterDelay(float duration, Vector3 impactPoint)
    {
        applyingKnockBack = true;
        yield return new WaitForSeconds(delayBeforeKnockback);
        // Calculate the knockback direction (from target to impact point)
        Vector3 knockbackDirection = (transform.position - impactPoint).normalized;

        // Apply the force
        if (rb != null)
        {
            rb.AddForce(knockbackDirection * forceMagnitude, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector3.zero; // Stop the target's movement
        rb.angularVelocity = 0; // Stop the target's rotation
        applyingKnockBack = false;
    }

}