using UnityEngine;

public class KeepRelativeOffsetAI : EnemyMovementAIEngine
{
    float randomizerTimer;
    float currentSpeed;
    Vector2 currentOffset;
    public KeepRelativeOffsetAI(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c) : base(rb2D, go, t, bounds, c)
    {
        currentSpeed = config.MoveSpeed;
        currentOffset = config.RelativeOffset;
    }

    public override void Update()
    {
        if (applyingKnockBack) return;

        randomizerTimer -= Time.deltaTime;
        if (randomizerTimer <= 0)
        {
            randomizerTimer = ApplyValueDeviation(8f);
            currentSpeed = ApplyValueDeviation(config.MoveSpeed);
            currentOffset.x *= Random.Range(0, 2) * 2 - 1;
            currentOffset.y *= Random.Range(0, 2) * 2 - 1;
        }

        Vector2 desiredPosition = (Vector2)target.position + currentOffset;
        if (Vector2.Distance(desiredPosition, gameObject.transform.position) < 0.01f) return;

        Vector2 direction = (desiredPosition - (Vector2)gameObject.transform.position).normalized;
        Vector2 newPos = rb.position + direction * currentSpeed * Time.deltaTime;
        rb.MovePosition(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos));
    }
}
