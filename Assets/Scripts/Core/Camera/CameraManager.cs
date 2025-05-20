using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Inbound Communication")]
    [SerializeField] CameraConfigEventChannelSO cameraConfigEventChannel;

    [Header("Camera Settings")]
    [SerializeField] CinemachineCamera lookAtMainCamera;
    [SerializeField] CinemachineCamera followMainCamera;
    [SerializeField] TransformEventChannelSO followTargetEventChannel;
    [SerializeField] Transform defaultFollowTarget;
    [SerializeField] bool hasFollowTarget;
    [SerializeField] bool shouldClampUsingBounds;

    CameraConfig currentCameraConfig;
    Transform followTarget;
    CinemachineConfiner2D followCameraConfiner;

    void Start()
    {
        if (followMainCamera == null || lookAtMainCamera == null)
        {
            throw new UnityException("Cameras not assigned in Camera Manager Children");
        }

        if (!followMainCamera.TryGetComponent<CinemachineConfiner2D>(out followCameraConfiner))
        {
            throw new UnityException("Confiber2D not assigned in Follow Camera Game Object");
        }

        followMainCamera.Priority = -1;
        lookAtMainCamera.Priority = 0;
    }

    void OnEnable()
    {
        Helpers.SubscribeIfNotNull(cameraConfigEventChannel, OnCameraConfig);
        Helpers.SubscribeIfNotNull(followTargetEventChannel, OnFollowTarget);
    }

    void OnDisable()
    {
        Helpers.UnsubscribeIfNotNull(followTargetEventChannel, OnFollowTarget);
        Helpers.UnsubscribeIfNotNull(cameraConfigEventChannel, OnCameraConfig);
    }

    void OnFollowTarget(Transform target)
    {
        followTarget = target;

        if (hasFollowTarget && followTarget != null)
        {
            followMainCamera.Follow = followTarget;
            followMainCamera.Priority = 0;
            lookAtMainCamera.Priority = -1;
        }
        else
        {
            followMainCamera.Follow = defaultFollowTarget;
            followMainCamera.Priority = -1;
            lookAtMainCamera.Priority = 0;
        }
    }

    void OnCameraConfig(CameraConfig config)
    {
        currentCameraConfig = config;
        AdjustCamera();
    }

    void AdjustCamera()
    {
        lookAtMainCamera.Lens.OrthographicSize = currentCameraConfig.OrthographicSize;
        followMainCamera.Lens.OrthographicSize = currentCameraConfig.OrthographicSize;
        defaultFollowTarget.position = currentCameraConfig.CameraPosition;
        followCameraConfiner.BoundingShape2D = currentCameraConfig.CameraBounds;
    }

}