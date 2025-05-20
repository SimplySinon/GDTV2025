using UnityEngine;

public abstract class EnemyMovementAIEngine
{
    public Transform target;
    public Collider2D movementBounds;
    protected Rigidbody2D rb;
    protected GameObject gameObject;
    protected EnemyConfigSO config;


    // Knockback settings
    float knockackForceMagnitude;
    float knockbackDuration;
    float delayBeforeKnockback;
    protected bool applyingKnockBack;
    protected Vector2 targetPosition;

    public EnemyMovementAIEngine(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c)
    {
        rb = rb2D;
        gameObject = go;
        target = t;
        movementBounds = bounds;
        config = c;

        if (gameObject == null) throw new UnityException("GameObject not assigned in movement engine");
        if (rb == null) throw new UnityException("Rigidbody not assigned in movement engine");
        if (target == null) throw new UnityException("Target not assigned in movement engine");
        if (movementBounds == null) throw new UnityException("Movement bounds not assigned in movement engine");
    }

    public void InitializeKnockBack(float forceMagnitude, float duration, float delay)
    {
        knockackForceMagnitude = forceMagnitude;
        knockbackDuration = duration;
        delayBeforeKnockback = delay;
    }

    protected bool IsInsideBounds(Vector2 position)
    {
        return movementBounds.OverlapPoint(position);
    }

    protected Vector2 ClampToBounds(Vector2 position)
    {
        Bounds bounds = movementBounds.bounds;
        return new Vector2(
            Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(position.y, bounds.min.y, bounds.max.y)
        );
    }

    public abstract void Update();

    public System.Collections.IEnumerator StopKnockbackAfterDelay()
    {
        applyingKnockBack = true;
        yield return new WaitForSeconds(delayBeforeKnockback);
        // Calculate the knockback direction (from target to impact point)
        Vector3 knockbackDirection = (gameObject.transform.position - target.position).normalized;

        // Apply the force
        if (rb != null)
        {
            Vector2 newPos = knockbackDirection * knockackForceMagnitude;
            rb.AddForce(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector3.zero; // Stop the target's movement
        rb.angularVelocity = 0; // Stop the target's rotation
        applyingKnockBack = false;
    }

    public bool IsApplyingKnockBack() { return applyingKnockBack; }

    protected float ApplyValueDeviation(float value)
    {
        return value + Random.Range(-config.ValueDiviation, config.ValueDiviation);
    }
}