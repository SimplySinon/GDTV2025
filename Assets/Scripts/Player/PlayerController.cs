using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Outbound Communication")]
    [SerializeField] private CharacterStateEventChannelSO playerStateEventChannel;
    [SerializeField] private Vector2EventChannelSO playerPositionEventChannel;
    [SerializeField] private TransformEventChannelSO playerTransformEventChannel;
    [SerializeField] private PlayerAttackEventChannelSO playerAttackEventChannel;

    [Header("Inbound Communication")]
    [SerializeField] private Vector2EventChannelSO moveDirectionEventChannel;
    [SerializeField] private BoolEventChannelSO attackInputEventChannel;
    [SerializeField] private BoolEventChannelSO defenseInputEventChannelSO;
    [SerializeField] private BoolEventChannelSO rangedAttackInputEventChannelSO;
    [SerializeField] private FloatEventChannelSO enemyAttackEventChannel;
    [SerializeField] private RoomConfigEventChannelSO roomConfigEventChannel;


    [Space, Header("Character Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] float cooldown;


    [Space, Header("Character Details")]
    [SerializeField] public float health = 100;
    [SerializeField] float meleeAttackDamage = 10;
    [SerializeField] float rangedAttackDamage = 5;
    [SerializeField] private Projectile playerRangedProjectile;
    [SerializeField] float projectileFireRate;
    [SerializeField] float projectileSpeed;


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
    [SerializeField] private BoxCollider2D defendCollider;
    [SerializeField] private Vector2 downAttackPosition;
    [SerializeField] private Vector2 upAttackPosition;
    [SerializeField] private Vector2 leftAttackPosition;
    [SerializeField] private Vector2 rightAttackPosition;

    private float timer;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private State currentState;
    private bool isAttackInput = false;
    private bool isDefendingInput = false;
    private bool isRangedAttackInput = false;
    private List<State> activeActions;


    float nextRangedProjectile;
    RoomConfig currentRoom;
    Vector2 previousMoveInput;

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
        Helpers.RaiseIfNotNull(playerTransformEventChannel, transform);

        activeActions = new List<State>()
        {
            State.Attacking,
            State.Defending,
        };

        nextRangedProjectile = Time.time;
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(moveDirectionEventChannel, OnMoveDirection);
        Helpers.SubscribeIfNotNull(attackInputEventChannel, OnAttackInput);
        Helpers.SubscribeIfNotNull(defenseInputEventChannelSO, OnDefenseInput);
        Helpers.SubscribeIfNotNull(rangedAttackInputEventChannelSO, OnRangedAttackInput);
        Helpers.SubscribeIfNotNull(enemyAttackEventChannel, OnEnemyAttack);
        Helpers.SubscribeIfNotNull(roomConfigEventChannel, OnPlayerEnterRoom);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(moveDirectionEventChannel, OnMoveDirection);
        Helpers.UnsubscribeIfNotNull(attackInputEventChannel, OnAttackInput);
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isDead) return; // ← Evita cualquier movimiento si está muerto

        if (currentState == State.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
            attackCollider.enabled = true;
            defendCollider.enabled = false;
            return;
        }
        else
        {
            attackCollider.enabled = false;
        }

        if (currentState == State.Defending)
        {
            rb.linearVelocity = Vector2.zero;
            defendCollider.enabled = true;
            attackCollider.enabled = false;
            return;
        }
        else
        {
            defendCollider.enabled = false;
        }

        Move();
        Helpers.RaiseIfNotNull(playerPositionEventChannel, transform.position);
    }

    private void OnPlayerEnterRoom(RoomConfig roomConfig)
    {
        currentRoom = roomConfig;
    }

    private bool isDead = false;

    private void OnEnemyAttack(float attackDamage)
    {
        if (isDead || currentState == State.Defending)
            return;

        health = Mathf.Clamp(health - attackDamage, 0, 100);

        if (currentState != State.Defending)
        {
            Vector2 knockbackDirection = -(previousMoveInput != Vector2.zero ? previousMoveInput.normalized : Vector2.up);
            rb.AddForce(knockbackDirection * 200f, ForceMode2D.Impulse);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        isDead = true;
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        Helpers.RaiseIfNotNull(playerStateEventChannel, new(currentState));

        // Apagamos colisiones y l�gica
        bodyCollider.enabled = false;
        hitboxCollider.enabled = false;
        attackCollider.enabled = false;
        defendCollider.enabled = false;

        // Esperamos 1 segundo antes de reaparecer
        Invoke(nameof(Respawn), 1f);
    }
    public void ResetState()
    {
        health = 100;
        isDead = false;
        rb.isKinematic = false;
        bodyCollider.enabled = true;
        hitboxCollider.enabled = true;
        attackCollider.enabled = false;
        defendCollider.enabled = false;
        currentState = State.Idle;
        Helpers.RaiseIfNotNull(playerStateEventChannel, new(currentState));
    }

    private void Respawn()
    {
        PlayerSpawnManager.Instance.RespawnPlayer(gameObject);
    }

    private void OnAttackInput(bool attackInput)
    {
        isAttackInput = attackInput;
        RaiseEvents();
    }

    private void OnDefenseInput(bool defenseInput)
    {
        isDefendingInput = defenseInput;
        RaiseEvents();
    }

    private void OnRangedAttackInput(bool rangedAttackInput)
    {
        isRangedAttackInput = rangedAttackInput;
        RaiseEvents();
    }

    private void OnMoveDirection(Vector2 moveDirection)
    {
        if (moveInput != Vector2.zero) previousMoveInput = moveInput;
        moveInput = moveDirection;

        RepositionColliders();
        RaiseEvents();
    }

    private void RaiseEvents()
    {
        RaiseEventOnce(State.Idle);

        if (isDefendingInput && timer <= 0)
        {
            currentState = State.Defending;
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(currentState));
            return;
        }

        if (isAttackInput && timer <= 0)
        {
            currentState = State.Attacking;
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(currentState));
            timer = cooldown;
            return;
        }

        if (isRangedAttackInput && timer <= 0)
        {
            currentState = State.RangedAttack;
            AttackFromRange();
            return;
        }

        if (moveInput.magnitude > 0.1f)
        {
            currentState = State.Moving;
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(State.Moving));
            return;
        }

        // Si no hay acciones activas, quedate en Idle
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

    private void RepositionColliders()
    {
        float x = moveInput.x;
        float y = moveInput.y;

        if (y < 0)
        {
            attackCollider.transform.localPosition = downAttackPosition;
            defendCollider.transform.localPosition = downAttackPosition;
        }
        else if (y > 0)
        {
            attackCollider.transform.localPosition = upAttackPosition;
            defendCollider.transform.localPosition = upAttackPosition;
        }
        else if (y == 0 && x < 0)
        {
            attackCollider.transform.localPosition = leftAttackPosition;
            defendCollider.transform.localPosition = leftAttackPosition;
        }
        else if (y == 0 && x > 0)
        {
            attackCollider.transform.localPosition = rightAttackPosition;
            defendCollider.transform.localPosition = rightAttackPosition;
        }
    }

    void AttackFromRange()
    {
        if (nextRangedProjectile <= Time.time)
        {
            Projectile projectile = Instantiate(playerRangedProjectile, attackCollider.transform.position, attackCollider.transform.rotation);
            projectile.transform.parent = null;
            float speed = projectileSpeed;
            Helpers.RaiseIfNotNull(playerStateEventChannel, new(State.RangedAttack));
            Vector2 dir = previousMoveInput;
            projectile.Initialize(speed, dir, attackable, currentRoom.RoomBounds, RangedDamageEnemy);
            nextRangedProjectile = Time.time + projectileFireRate;
        }
    }

    void RangedDamageEnemy(GameObject enemy)
    {
        EnemyController controller = enemy.GetComponent<EnemyController>();
        Helpers.RaiseIfNotNull(playerAttackEventChannel, new(controller.GetEnemyId(), rangedAttackDamage, true));
    }

    void MeleeDamageEnemy(GameObject enemy)
    {
        EnemyController controller = enemy.GetComponent<EnemyController>();
        Helpers.RaiseIfNotNull(playerAttackEventChannel, new(controller.GetEnemyId(), meleeAttackDamage));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (attackable.Contains(other.gameObject.layer) && currentState == State.Attacking)
        {
            MeleeDamageEnemy(other.gameObject);
        }
 
        if (other.gameObject.CompareTag("Dead"))
        {
            Die();
        }
        if (other.gameObject.CompareTag("End"))
        {
            SceneManager.LoadScene("Credits");
        }
    }


}
