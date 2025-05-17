using UnityEditor;
using UnityEngine;

public class Vector2ListenerExample : MonoBehaviour
{
    [Header("Inbound Communication")]
    public Vector2EventChannelSO positionEmitterEventChannel;

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(positionEmitterEventChannel, OnPosition);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(positionEmitterEventChannel, OnPosition);
    }

    void OnPosition(Vector2 position)
    {
        Debug.Log(position);
    }
}