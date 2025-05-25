using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;
    public Vector3 lastCheckpointPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameObject spawnPoint = GameObject.FindWithTag("PlayerSpawn");
            if (spawnPoint != null)
                lastCheckpointPosition = spawnPoint.transform.position;
            else
                Debug.LogError("No se encontró objeto con tag 'PlayerSpawn'");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCheckpoint(Vector3 newPosition)
    {
        lastCheckpointPosition = newPosition;
    }
    

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = lastCheckpointPosition;
        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.ResetState();
        else
            Debug.LogError("El jugador no tiene PlayerController para resetear estado.");
    }
}
