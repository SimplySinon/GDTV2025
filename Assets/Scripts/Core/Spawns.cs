using UnityEngine;

public class Spawns : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform spawnPoint;  

    void Start()
    {
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
