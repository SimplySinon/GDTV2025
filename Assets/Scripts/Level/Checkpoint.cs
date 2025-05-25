using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                PlayerSpawnManager.Instance.UpdateCheckpoint(transform.position);
                Debug.Log("Checkpoint actualizado");

                playerController.health = 100;
                playerController.ResetState(); 
            }
        }
    }
}
