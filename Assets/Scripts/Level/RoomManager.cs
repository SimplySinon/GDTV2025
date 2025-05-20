using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Outbound Communication")]
    [SerializeField] CameraConfigEventChannelSO cameraConfigEventChannel;

    [Header("Room Camera Config")]
    [SerializeField] CameraConfig cameraConfig;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool useRoomPosition;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (playerLayer.Contains(other.gameObject.layer))
            OnRoomTriggerEnter();
    }

    public void OnRoomTriggerEnter()
    {
        if (useRoomPosition)
            cameraConfig.CameraPosition = transform.position;

        Helpers.RaiseIfNotNull(cameraConfigEventChannel, cameraConfig);
    }

}