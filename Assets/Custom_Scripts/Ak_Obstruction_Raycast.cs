using UnityEngine;
using AK.Wwise; // Se añade si usas AkSoundEngine y no está implícito. Generalmente solo es necesario 'using UnityEngine;'

public class Ak_Obstruction_Raycast : MonoBehaviour
{
    // VARIABLES PÚBLICAS (SE VEN EN EL INSPECTOR)
    // ---
    public string obstructionRTPCName = "ObstructionAmount";
    public float maxDistance = 30f;
    public float updateFrequency = 0.2f;

    // Esta variable SÍ se debe ver como un campo de selección de capas en el Inspector.
    public LayerMask obstructionMask;

    // VARIABLES PRIVADAS
    // ---
    private Transform audioListener;
    private float nextUpdateTime;

    void Start()
    {
        // En VR, la cámara principal es el listener
        audioListener = Camera.main.transform;
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            CalculateObstruction();
            nextUpdateTime = Time.time + updateFrequency;
        }
    }

    // **********************************************
    // ¡AQUÍ DEBE ESTAR LA FUNCIÓN! (DENTRO DE LA CLASE)
    // **********************************************
    void CalculateObstruction()
    {
        // Dirección del rayo: desde el TV hacia la cámara del jugador
        Vector3 directionToListener = audioListener.position - transform.position;
        float distanceToListener = directionToListener.magnitude;

        if (distanceToListener > maxDistance)
        {
            AkSoundEngine.SetRTPCValue(obstructionRTPCName, 0f, gameObject);
            return;
        }

        // 1. Trazar el Rayo:
        RaycastHit hit;
        float currentObstructionValue = 0f;

        // Si el Raycast GOLPEA algo en la capa de 'obstructionMask'
        if (Physics.Raycast(transform.position, directionToListener, out hit, distanceToListener, obstructionMask))
        {
            currentObstructionValue = 75f; // Valor fijo para oclusión
        }

        // 2. Enviar el RTPC a Wwise:
        AkSoundEngine.SetRTPCValue(obstructionRTPCName, currentObstructionValue, gameObject);

        // DEBUGGING: Visualiza el Rayo en el editor.
        Debug.DrawRay(transform.position, directionToListener, currentObstructionValue > 0 ? Color.red : Color.green);
    }
}