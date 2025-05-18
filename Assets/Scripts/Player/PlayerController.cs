using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveDirectionEventChannel;
    [SerializeField] private CharacterStateEventChannelSO playerStateEventChannel;
    [SerializeField] private float speed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D circleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        // Remove Gravity Effects
        rb.gravityScale = 0;
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(moveDirectionEventChannel, OnMoveDirection);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(moveDirectionEventChannel, OnMoveDirection);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Move();
    }

    private void OnMoveDirection(Vector2 moveDirection)
    {
        moveInput = moveDirection;

        if (moveInput.magnitude > 0.1f || moveInput.magnitude < -0.1f)
        {
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(State.Moving));
        }
        else
        {
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(State.Idle));
        }
    }

    private void Move()
    {
        rb.linearVelocity = moveInput * speed;


    }
}
