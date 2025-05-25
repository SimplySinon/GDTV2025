using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishCredits : MonoBehaviour
{
   
    void Start()
    {
        Invoke("finishCredits", 11);
    }


    void Update()
    {
        
    }
    public void finishCredits()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
