using System;
using UnityEditor;
using UnityEngine;

public class Vector2EmitterExample : MonoBehaviour
{
    [Header("Outbound Communication")]
    public Vector2EventChannelSO positionEventChannel;

    [Header("Movement Settings")]
    public float speed = 2f;              // Movement speed
    public float changeDirectionTime = 2f; // Time in seconds before changing direction

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChooseNewDirection();
        timer = changeDirectionTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ChooseNewDirection();
            timer = changeDirectionTime;
        }

        Helpers.RaiseIfNotNull(positionEventChannel, transform.position);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    void ChooseNewDirection()
    {
        // Get a random direction with unit length
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
    }

}