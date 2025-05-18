using UnityEngine;
using UnityEngine.InputSystem;

public class InputEmitterExample : MonoBehaviour
{
    [Header("Outbound Communication")]
    //This takes a scriptable object of type Vector2EventChannelSO to send Vector2 Events
    [SerializeField] Vector2EventChannelSO moveInputEventChannel;
    [SerializeField] BoolEventChannelSO jumpInputEventChannel;
    InputAction moveAction;
    InputAction jumpAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // Raising an event of type Vector2
        Helpers.RaiseIfNotNull(moveInputEventChannel, moveValue);

        //Raise a bool event to represent the Jump key being pressed
        Helpers.RaiseIfNotNull(jumpInputEventChannel, jumpAction.IsPressed());
    }
}