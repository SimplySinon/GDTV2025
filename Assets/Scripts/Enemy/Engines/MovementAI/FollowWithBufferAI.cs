using UnityEngine;

public class FollowWithBufferAI : EnemyMovementAIEngine
{
    private float desiredDistance;
    private float distanceChangeInterval = 5f;
    private float distanceChangeTimer;
    private float directionChangeTimer;
    private Vector2 dir;
    public FollowWithBufferAI(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c) : base(rb2D, go, t, bounds, c)
    {
        SetNewDistance();
    }

    private void SetNewDistance()
    {
        desiredDistance = Random.Range(config.BufferMinDistance, config.BufferMaxDistance);
        desiredDistance = ApplyValueDeviation(desiredDistance);
        distanceChangeTimer = ApplyValueDeviation(distanceChangeInterval);
    }

    public override void Update()
    {
        if (applyingKnockBack || isAttacking) return;

        float currentDistance = Vector2.Distance(gameObject.transform.position, target.position);

        distanceChangeTimer -= Time.deltaTime;
        if (distanceChangeTimer <= 0)
            SetNewDistance();


        directionChangeTimer -= Time.deltaTime;

        if (directionChangeTimer <= 0)
        {
            if (currentDistance > desiredDistance)
            {
                dir = (target.position - gameObject.transform.position).normalized;
            }
            else if (currentDistance < desiredDistance)
            {
                dir = (gameObject.transform.position - target.position).normalized;
            }
            directionChangeTimer = 0.25f;
        }

        Vector2 newPos = rb.position + dir * config.FollowSpeed * Time.deltaTime;
        rb.MovePosition(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos));
    }
}
