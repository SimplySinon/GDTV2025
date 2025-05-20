using UnityEngine;

[System.Serializable]
public class CameraConfig
{
    public float OrthographicSize;
    public Vector2 CameraPosition;
    public Collider2D CameraBounds;

    public CameraConfig(float size, Vector2 pos, Collider2D bounds)
    {
        OrthographicSize = size;
        CameraPosition = pos;
        CameraBounds = bounds;
    }
}