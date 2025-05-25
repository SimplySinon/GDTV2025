using Unity.VisualScripting;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pause;
    public bool paused=false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) {
                Resume();
            }

            else
            {
                paused = true;
            }
        }

    }
    public void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1;
        paused = false;
    }
    public void Stop()
    {
        pause.SetActive(true);
        Time.timeScale = 0;
        paused = true;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
