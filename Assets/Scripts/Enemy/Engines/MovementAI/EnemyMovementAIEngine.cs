using UnityEngine;
using UnityEngine.Events;

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
    protected bool isAttacking;
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
        if (movementBounds is PolygonCollider2D)
        {
            return ClampToPolygon(position);
        }

        return ClampToBoxCollider(position);
    }
    protected Vector2 ClampToBoxCollider(Vector2 position)
    {
        Bounds bounds = movementBounds.bounds;
        return new Vector2(
            Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(position.y, bounds.min.y, bounds.max.y)
        );
    }


    protected Vector2 ClampToPolygon(Vector2 position)
    {
        PolygonCollider2D polygonCollider = (PolygonCollider2D)movementBounds;

        if (polygonCollider.OverlapPoint(position))
        {
            // Point is inside polygon, return as-is
            return position;
        }

        // Point is outside, find the closest point on the polygon's edges
        Vector2 closestPoint = position;
        float minDistance = float.MaxValue;

        for (int p = 0; p < polygonCollider.pathCount; p++)
        {
            Vector2[] points = polygonCollider.GetPath(p);
            int count = points.Length;

            for (int i = 0; i < count; i++)
            {
                Vector2 a = polygonCollider.transform.TransformPoint(points[i]);
                Vector2 b = polygonCollider.transform.TransformPoint(points[(i + 1) % count]);

                Vector2 closest = ClosestPointOnSegment(position, a, b);
                float distance = Vector2.SqrMagnitude(position - closest);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closest;
                }
            }
        }

        return closestPoint;
    }

    private Vector2 ClosestPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

    public abstract void Update();

    public System.Collections.IEnumerator StopKnockbackAfterDelay(UnityAction callback)
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
        callback();
    }

    public bool IsApplyingKnockBack() { return applyingKnockBack; }

    protected float ApplyValueDeviation(float value)
    {
        return value + Random.Range(-config.ValueDiviation, config.ValueDiviation);
    }

    public void SetIsAttacking(bool attacking)
    {
        isAttacking = attacking;
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }
}