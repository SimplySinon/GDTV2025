using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishIntro : MonoBehaviour
{
    void Start()
    {
        Invoke("finishIntro", 10);
    }


    public void finishIntro()
    {
        SceneManager.LoadScene("Main");
    }
}
