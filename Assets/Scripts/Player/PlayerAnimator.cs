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

    void Start()
    {
        animator = GetComponent<Animator>();
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
        animator.SetBool("isMoving", playerState == State.Moving);
        animator.SetBool("isAttacking", playerState == State.Attacking);

        animator.SetFloat("x", moveInput.x);
        animator.SetFloat("y", moveInput.y);
    }
}