using UnityEngine;
using UnityEngine.Events;

public class PostureDetector : MonoBehaviour
{
    public float standingThreshold = 1.5f; // Altura mínima para "de pie" (en metros)
    public UnityEvent OnStand; // Evento público para postura de pie
    public UnityEvent OnSit;   // Evento público para postura sentado

    void Update()
    {
        float headHeight = Camera.main.transform.position.y;

        if (headHeight >= standingThreshold)
        {
            OnStand.Invoke(); // Dispara eventos para postura de pie
        }
        else
        {
            OnSit.Invoke(); // Dispara eventos para postura sentado
        }
    }
}
