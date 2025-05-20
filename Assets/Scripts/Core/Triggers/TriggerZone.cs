using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] bool isOneShot;
    [SerializeField] LayerMask canBeTriggeredBy;
    [SerializeField] UnityEvent onTriggerEnter;

    [Space, Header("Trigger Events")]
    [SerializeField] UnityEvent onTriggerExit;

    private bool alreadyEntered = false;
    private bool alreadyExited = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyEntered) return;
        if (!canBeTriggeredBy.Contains(other.gameObject.layer)) return;

        onTriggerEnter?.Invoke();

        if (isOneShot) alreadyEntered = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (alreadyExited) return;
        if (!canBeTriggeredBy.Contains(other.gameObject.layer)) return;

        onTriggerExit?.Invoke();

        if (isOneShot) alreadyExited = true;
    }
}