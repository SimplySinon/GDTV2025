using UnityEngine;

public class Example : MonoBehaviour
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
        Debug.Log(moveDirection);
    }

    void OnJumpInput(bool jumpInput)
    {
        Debug.Log(jumpInput);
    }
}