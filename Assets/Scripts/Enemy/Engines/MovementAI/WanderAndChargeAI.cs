using UnityEngine;

public class WanderAndChargeAI : EnemyMovementAIEngine
{
    private Vector2 wanderDirection;
    private float wanderChangeInterval = 2f;
    private float wanderTimer;

    public WanderAndChargeAI(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c) : base(rb2D, go, t, bounds, c)
    {
    }

    public override void Update()
    {
        if (applyingKnockBack) return;

        float distance = Vector2.Distance(gameObject.transform.position, target.position);

        if (distance <= config.ProximityRange)
        {
            Vector2 dir = (target.position - gameObject.transform.position).normalized;
            Vector2 newPos = rb.position + dir * config.ChargeSpeed * Time.deltaTime;
            rb.MovePosition(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos));
        }
        else
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                wanderDirection = Random.insideUnitCircle.normalized;
                wanderTimer = ApplyValueDeviation(wanderChangeInterval);
            }

            Vector2 newPos = rb.position + wanderDirection * config.WanderSpeed * Time.deltaTime;
            rb.MovePosition(IsInsideBounds(newPos) ? newPos : ClampToBounds(newPos));
        }
    }
}
