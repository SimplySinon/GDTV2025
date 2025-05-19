using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Outbound Communication")]
    [SerializeField] private Vector2EventChannelSO moveDirectionEventChannel;
    [SerializeField] private CharacterStateEventChannelSO playerStateEventChannel;
    [SerializeField] private Vector2EventChannelSO playerPositionEventChannel;
    [SerializeField] private BoolEventChannelSO attackInputEventChannel;
    [SerializeField] private IntEventChannelSO enemyIdEventChannel;

    [Space, Header("Character Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] float attackCooldown;

    [Space, Header("Character Collision Settings")]
    [SerializeField] private CircleCollider2D hitboxCollider;
    [SerializeField] private CapsuleCollider2D bodyCollider;
    [SerializeField] private Vector2 bodyColliderSize = new(1.5f, 1);
    [SerializeField] private Vector2 bodyColliderOffset = new(0, 0.5f);
    [SerializeField] private float hitboxColliderRadius = 1;
    [SerializeField] private Vector2 hitboxColliderOffset = new(0, 1);


    [Space, Header("Attack Collider Settings")]
    [SerializeField] private LayerMask attackable;
    [SerializeField] private BoxCollider2D attackCollider;
    [SerializeField] private Vector2 downAttackPosition;
    [SerializeField] private Vector2 upAttackPosition;
    [SerializeField] private Vector2 leftAttackPosition;
    [SerializeField] private Vector2 rightAttackPosition;

    private float attackTimer;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private State currentState;
    private bool isAttackingInput = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bodyCollider == null || hitboxCollider == null)
        {
            throw new UnityException("Player Colliders not Assigned");
        }

        // Configure Hitbox Collider
        hitboxCollider.offset = hitboxColliderOffset;
        hitboxCollider.radius = hitboxColliderRadius;

        // Configure Body Collider
        bodyCollider.offset = bodyColliderOffset;
        bodyCollider.size = bodyColliderSize;

        // Configure Attack Collider
        attackCollider.includeLayers = attackable;

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
            attackCollider.enabled = true;
            return;
        }
        else
        {
            attackCollider.enabled = false;
        }

        Move();
        Helpers.RaiseIfNotNull(playerPositionEventChannel, transform.position);
    }

    private void OnAttackInput(bool attackInput)
    {
        isAttackingInput = attackInput;
        RaiseEvents();
    }

    private void OnMoveDirection(Vector2 moveDirection)
    {
        moveInput = moveDirection;
        RepositionAttackCollider();
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

    private void RepositionAttackCollider()
    {
        float x = moveInput.x;
        float y = moveInput.y;

        if (y < 0)
        {
            attackCollider.transform.localPosition = downAttackPosition;
        }
        else if (y > 0)
        {
            attackCollider.transform.localPosition = upAttackPosition;
        }
        else if (y == 0 && x < 0)
        {
            attackCollider.transform.localPosition = leftAttackPosition;
        }
        else if (y == 0 && x > 0)
        {
            attackCollider.transform.localPosition = rightAttackPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (attackable.Contains(other.gameObject.layer))
        {
            Debug.Log($"Attacking: {other.gameObject.name}");
            EnemyController controller = other.GetComponent<EnemyController>();
            Helpers.RaiseIfNotNull(enemyIdEventChannel, controller.GetEnemyId());
        }
    }
}
