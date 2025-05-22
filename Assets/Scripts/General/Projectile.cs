using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    float speed;
    Vector2 projectileDirection;
    UnityAction onHitCallback;
    LayerMask targetMask;
    Collider2D projectileBounds;
    Collider2D projectileCollider;

    Vector2 velocity;
    Vector2 acceleration = new Vector2(0, 0); // Acceleration (e.g. gravity)

    public void Initialize(float projectileSpeed, Vector2 direction, LayerMask target, Collider2D bounds, UnityAction onHit)
    {
        speed = projectileSpeed;
        projectileDirection = direction.normalized;
        onHitCallback = onHit;
        targetMask = target;

        projectileBounds = bounds;
        projectileCollider = GetComponent<Collider2D>();
        projectileCollider.includeLayers = target;
    }

    void OnDestroy()
    {
        //Spawn Exit Particles e.g explosion, sounds
    }

    void Update()
    {
        float dt = Time.deltaTime;
        velocity = projectileDirection * speed;

        // Move the object
        transform.position += (Vector3)(velocity * dt);

        // Destroy if outside bounds
        if (!IsInsideBounds())
        {
            Destroy(gameObject);
        }
    }

    bool IsInsideBounds()
    {
        if (projectileBounds == null) return true;

        Bounds bounds = projectileBounds.bounds;
        Vector2 pos = transform.position;
        return bounds.Contains(pos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetMask.Contains(other.gameObject.layer))
        {
            onHitCallback();
            Destroy(gameObject);
        }
    }
}