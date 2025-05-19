using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO moveDirectionEventChannel;
    [SerializeField] private CharacterStateEventChannelSO playerStateEventChannel;
    [SerializeField] private BoolEventChannelSO attackInputEventChannel;
    [SerializeField] private float speed = 5f;
    [SerializeField] float attackCooldown;

    private float attackTimer;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D circleCollider;
    private State currentState;
    private bool isAttackingInput = false;

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
        Helpers.SubscribeIfNotNull(attackInputEventChannel, OnAttackInput);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(moveDirectionEventChannel, OnMoveDirection);
        Helpers.UnsubscribeIfNotNull(attackInputEventChannel, OnAttackInput);
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (currentState == State.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Move();
    }

    private void OnAttackInput(bool attackInput)
    {
        isAttackingInput = attackInput;
        RaiseEvents();
    }

    private void OnMoveDirection(Vector2 moveDirection)
    {
        moveInput = moveDirection;
        RaiseEvents();
    }

    private void RaiseEvents()
    {
        if (isAttackingInput && attackTimer <= 0)
        {
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(State.Attacking));
            currentState = State.Attacking;
            attackTimer = attackCooldown;
            return;
        }

        if (moveInput.magnitude > 0.1f || moveInput.magnitude < -0.1f)
        {
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(State.Moving));
            currentState = State.Moving;
            return;
        }

        if (!isAttackingInput)
            RaiseEventOnce(State.Idle);
    }

    private void RaiseEventOnce(State state)
    {
        if (currentState != state)
        {
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(state));
            currentState = state;
        }
    }

    private void Move()
    {
        rb.linearVelocity = moveInput * speed;
    }
}
