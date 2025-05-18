using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveDirectionEventChannel;
    [SerializeField] private float speed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D circleCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
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
    private void Update()
    {
        Move();
    }

    private void OnMoveDirection(Vector2 moveDirection)
    {
        moveInput = moveDirection;
    }

    private void Move()
    {
        rb.linearVelocity = moveInput * speed;
    }

    // public void OnMove(InputValue value)
    // {
    //     moveInput = value.Get<Vector2>();
    // }
}
