using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] Vector2EventChannelSO moveDirectionEventChannelSO;
    [SerializeField] BoolEventChannelSO attackInputEventChannelSO;
    [SerializeField] BoolEventChannelSO defenseInputEventChannelSO;
    [SerializeField] BoolEventChannelSO rangedAttackInputEventChannelSO;

    InputAction moveAction;
    InputAction attackAction;
    InputAction rangedAttackAction;
    InputAction defenseAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        rangedAttackAction = InputSystem.actions.FindAction("RangedAttack");
        defenseAction = InputSystem.actions.FindAction("Defense");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Helpers.RaiseIfNotNull(moveDirectionEventChannelSO, moveValue);

        Helpers.RaiseIfNotNull(attackInputEventChannelSO, attackAction.IsPressed());
        Helpers.RaiseIfNotNull(defenseInputEventChannelSO, defenseAction.IsPressed());
        Helpers.RaiseIfNotNull(rangedAttackInputEventChannelSO, rangedAttackAction.IsPressed());

    }
}