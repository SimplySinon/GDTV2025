using UnityEngine;
using UnityEngine.UI;
public class Healthbar : MonoBehaviour
{
    public Image fill;
    private PlayerController playerController;
    private float FullHealth;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        FullHealth = playerController.health;
    }

    // Update is called once per frame
    void Update()
    {
        fill.fillAmount = playerController.health/FullHealth;
    }
}
