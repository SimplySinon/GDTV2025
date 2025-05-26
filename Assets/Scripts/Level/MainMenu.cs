using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Intro");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
