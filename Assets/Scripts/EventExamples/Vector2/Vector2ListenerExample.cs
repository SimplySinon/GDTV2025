using UnityEditor;
using UnityEngine;

public class Vector2ListenerExample : MonoBehaviour
{
    [Header("Inbound Communication")]
    public Vector2EventChannelSO positionEventChannel;

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(positionEventChannel, OnPosition);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(positionEventChannel, OnPosition);
    }

    void OnPosition(Vector2 position)
    {
        Debug.Log(position);
    }
}