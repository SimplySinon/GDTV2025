using UnityEngine;

public class ChargeAI : EnemyMovementAIEngine
{
    private float chargeTimer;
    private float chargeCooldownTimer;

    public ChargeAI(Rigidbody2D rb2D, GameObject go, Transform t, Collider2D bounds, EnemyConfigSO c) : base(rb2D, go, t, bounds, c)
    {
    }

    public override void Update()
    {
        if (applyingKnockBack || isAttacking) return;

        chargeTimer -= Time.deltaTime;
        if (chargeTimer >= 0)
        {

            Vector2 direction = (target.position - gameObject.transform.position).normalized;
            Vector2 newPos = rb.position + direction * config.ChargeSpeed * Time.deltaTime;

            if (IsInsideBounds(newPos))
                rb.MovePosition(newPos);
            else
                rb.MovePosition(ClampToBounds(newPos));

            chargeCooldownTimer = ApplyValueDeviation(config.ChargeCooldown);
            return;
        }

        chargeCooldownTimer -= Time.deltaTime;
        if (chargeCooldownTimer <= 0)
        {
            chargeTimer = ApplyValueDeviation(config.ChargeTime);
        }
    }
}