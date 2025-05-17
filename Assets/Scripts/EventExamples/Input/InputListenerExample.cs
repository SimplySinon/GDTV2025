using UnityEngine;

public class InputListenerExample : MonoBehaviour
{
    [Header("Inbound Communication")]
    [SerializeField] Vector2EventChannelSO moveInputEventChannel;
    [SerializeField] BoolEventChannelSO jumpInputEventChannel;

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(moveInputEventChannel, OnMoveInput);
        Helpers.SubscribeIfNotNull(jumpInputEventChannel, OnJumpInput);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(moveInputEventChannel, OnMoveInput);
        Helpers.UnsubscribeIfNotNull(jumpInputEventChannel, OnJumpInput);
    }

    void OnMoveInput(Vector2 moveDirection)
    {
        // Listening for move input
        if (moveDirection != Vector2.zero) Debug.Log(moveDirection);
    }

    void OnJumpInput(bool jumpInput)
    {
        // Listening for jump input
        if (jumpInput) Debug.Log(jumpInput);
    }
}