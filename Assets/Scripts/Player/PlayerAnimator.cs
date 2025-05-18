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
    }

    void OnMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        animator.SetBool("isMoving", playerState == State.Moving);
        if (playerState == State.Moving)
        {
            animator.SetFloat("x", moveInput.x);
            animator.SetFloat("y", moveInput.y);
        }
    }
}