using UnityEngine;

public class FollowWithBufferAI : EnemyMovementAIEngine
{
    private float desiredDistance;
    private float distanceChangeInterval = 5f;
    private float timer;

    public FollowWithBufferAI(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c) : base(rb2D, go, t, bounds, c)
    {
        SetNewDistance();
    }

    private void SetNewDistance()
    {
        desiredDistance = Random.Range(config.BufferMinDistance, config.BufferMaxDistance);
        desiredDistance = ApplyValueDeviation(desiredDistance);
        timer = ApplyValueDeviation(distanceChangeInterval);
    }

    public override void Update()
    {
        if (applyingKnockBack) return;

        float currentDistance = Vector2.Distance(gameObject.transform.position, target.position);

        timer -= Time.deltaTime;
        if (timer <= 0)
            SetNewDistance();

        Vector2 dir = Vector2.zero;
        if (currentDistance > desiredDistance)
        {
            dir = (target.position - gameObject.transform.position).normalized;
        }
        else if (currentDistance < desiredDistance)
        {
            dir = (gameObject.transform.position - target.position).normalized;
        }

        Vector2 newPos = rb.position + dir * config.FollowSpeed * Time.deltaTime;
        rb.MovePosition(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos));
    }
}
