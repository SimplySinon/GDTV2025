using UnityEngine;

public class RandomPatrolAI : EnemyMovementAIEngine
{
    private Vector2 patrolTarget;
    private float threshold = 0.5f;

    private float randomTimer;

    public RandomPatrolAI(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c) : base(rb2D, go, t, bounds, c)
    {
        SetNewPatrolPoint();
    }

    private void SetNewPatrolPoint()
    {
        Bounds bounds = movementBounds.bounds;
        patrolTarget = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }

    public override void Update()
    {
        if (applyingKnockBack || isAttacking) return;

        Vector2 currentPosition = gameObject.transform.position;
        randomTimer -= Time.deltaTime;
        if (Vector2.Distance(currentPosition, patrolTarget) < threshold || randomTimer <= 0)
        {
            SetNewPatrolPoint();
            randomTimer = ApplyValueDeviation(10);
        }

        Vector2 direction = (patrolTarget - currentPosition).normalized;
        Vector2 newPos = rb.position + direction * config.MoveSpeed * Time.deltaTime;
        rb.MovePosition(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos));
    }
}
