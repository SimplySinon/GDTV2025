using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] Vector2EventChannelSO moveDirectionEventChannelSO;

    InputAction moveAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Helpers.RaiseIfNotNull(moveDirectionEventChannelSO, moveValue);
    }

    public void OnMove(InputValue value)
    {
        Debug.Log(value.Get<Vector2>());
    }
}