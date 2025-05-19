using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] Vector2EventChannelSO moveDirectionEventChannelSO;
    [SerializeField] BoolEventChannelSO attackInputEventChannelSO;

    InputAction moveAction;
    InputAction attackAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Helpers.RaiseIfNotNull(moveDirectionEventChannelSO, moveValue);

        Helpers.RaiseIfNotNull(attackInputEventChannelSO, attackAction.IsPressed());

    }
}