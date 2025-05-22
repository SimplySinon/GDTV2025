using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Inbound Communication")]
    [SerializeField] Vector2EventChannelSO moveInputEventChannelSO;
    [SerializeField] CharacterStateEventChannelSO playerStateEventChannelSO;


    private Animator animator;
    private State playerState;
    private Vector2 moveInput;

    bool hasIsMoving, hasIsTakingDamage, hasIsDead, hasIsAttacking, hasIsRangedAttack, hasIsDefending;

    void Start()
    {
        animator = GetComponent<Animator>();

        hasIsMoving = animator.ContainsParam("isMoving");
        hasIsTakingDamage = animator.ContainsParam("isTakingDamage");
        hasIsDead = animator.ContainsParam("isDead");
        hasIsAttacking = animator.ContainsParam("isAttacking");
        hasIsRangedAttack = animator.ContainsParam("isRangedAttack");
        hasIsDefending = animator.ContainsParam("isDefending");
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(playerStateEventChannelSO, OnPlayerState);
        Helpers.SubscribeIfNotNull(moveInputEventChannelSO, OnMoveInput);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(playerStateEventChannelSO, OnPlayerState);
        Helpers.UnsubscribeIfNotNull(moveInputEventChannelSO, OnMoveInput);
    }

    void OnPlayerState(CharacterState state)
    {
        playerState = state.State;
        Animate();
    }

    void OnMoveInput(Vector2 input)
    {
        if (Vector2.zero != input)
            moveInput = input;
    }

    void Update()
    {
    }

    private void Animate()
    {
        if (hasIsMoving) animator.SetBool("isMoving", playerState == State.Moving);
        if (hasIsTakingDamage) animator.SetBool("isTakingDamage", playerState == State.TakingDamage);
        if (hasIsAttacking) animator.SetBool("isAttacking", playerState == State.Attacking);
        if (hasIsDefending) animator.SetBool("isDefending", playerState == State.Defending);
        if (hasIsRangedAttack) animator.SetBool("isRangeAttack", playerState == State.RangedAttack);
        if (hasIsDead) animator.SetBool("isDead", playerState == State.Dead);

        animator.SetFloat("x", moveInput.x);
        animator.SetFloat("y", moveInput.y);
    }
}