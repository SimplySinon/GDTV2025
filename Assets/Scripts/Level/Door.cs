using UnityEngine;

public class SpikesOpener : MonoBehaviour
{
    public GameObject[] Doors;
    public string enemyTag = "Enemy"; // Tag de los enemigos
    public string playerTag = "Player"; // Tag del jugador

    private bool playerEntered = false;
    private bool doorsOpened = false;

    void Update()
    {
        if (!playerEntered || doorsOpened)
            return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
        {
            OpenDoors();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Jugador entró a la sala. Activando control de enemigos.");
            playerEntered = true;
        }
    }

    void OpenDoors()
    {
        doorsOpened = true;
        Debug.Log("Todos los enemigos eliminados. Abriendo puertas...");

        foreach (GameObject door in Doors)
        {
            if (door == null) continue;

            Animator animator = door.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("Open", true);

            door.tag = "OpenDoor";

            Collider2D collider = door.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;
        }
    }
}
