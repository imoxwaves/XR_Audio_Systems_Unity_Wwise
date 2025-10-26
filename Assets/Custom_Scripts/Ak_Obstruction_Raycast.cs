using UnityEngine;
using AK.Wwise;

// No se necesita un 'using' adicional para AkSoundEngine en la mayoría de las integraciones
// modernas de Wwise/Unity, ya que la clase es globalmente accesible.

public class AkObstruction_Raycast : MonoBehaviour
{
    // VARIABLES PÚBLICAS (Aparecen en el Inspector)
    // ---
    public string obstructionRTPCName = "ObstructionAmount";
    public float maxDistance = 30f;
    public float updateFrequency = 0.2f;

    // ESTO GENERA el selector de capas en el Inspector
    public LayerMask obstructionMask;

    // VARIABLES PRIVADAS
    // ---
    private Transform audioListener;
    private float nextUpdateTime;

    void Start()
    {
        // En VR, la cámara principal es el listener (oyente)
        audioListener = Camera.main.transform;
    }

    void Update()
    {
        // Optimizacion: Solo corre la lógica del Raycast cada 0.2 segundos
        if (Time.time >= nextUpdateTime)
        {
            CalculateObstruction();
            nextUpdateTime = Time.time + updateFrequency;
        }
    }

    /// <summary>
    /// Utiliza doble Raycast para detectar Obstrucción (bloqueo parcial) u Oclusión (bloqueo total).
    /// Envía 0, 50 o 100 al RTPC de Wwise.
    /// </summary>
    void CalculateObstruction()
    {
        // 1. CÁLCULO DE VECTORES
        Vector3 sourcePosition = transform.position;
        Vector3 listenerPosition = audioListener.position;

        // Rayo del TV hacia el jugador
        Vector3 directionToListener = listenerPosition - sourcePosition;
        float distanceToListener = directionToListener.magnitude;

        // Si el jugador está fuera del rango de detección, envía 0 RTPC y termina.
        if (distanceToListener > maxDistance)
        {
            AkSoundEngine.SetRTPCValue(obstructionRTPCName, 0f, gameObject);
            return;
        }

        float obstructionValue = 0f;
        float occlusionValue = 0f;

        // 2. PRIMER RAYCAST: DETECCIÓN DE OBSTRUCCIÓN (Fuente -> Oyente)
        // Detecta cualquier bloqueo entre el TV y el Player.
        RaycastHit hit;

        if (Physics.Raycast(sourcePosition, directionToListener, out hit, distanceToListener, obstructionMask))
        {
            // Hay al menos un objeto bloqueando el sonido. Esto es Obstrucción (valor intermedio).
            obstructionValue = 50f;
        }

        // 3. SEGUNDO RAYCAST: DETECCIÓN DE OCLUSIÓN (Oyente -> Fuente)
        // Solo verificamos la Oclusión si ya detectamos Obstrucción para ahorrar CPU.
        if (obstructionValue > 0f)
        {
            // Rayo del jugador hacia el TV (dirección inversa)
            Vector3 directionToSource = sourcePosition - listenerPosition;

            if (Physics.Raycast(listenerPosition, directionToSource, out hit, distanceToListener, obstructionMask))
            {
                // Si el sonido está bloqueado en ambos extremos (ida y vuelta), lo consideramos OCLUSIÓN TOTAL.
                occlusionValue = 100f;
            }
        }

        // 4. DECISIÓN FINAL Y ENVÍO A WWISE
        // Prioriza el valor más alto: 100 > 50 > 0
        float finalRTPCValue = Mathf.Max(obstructionValue, occlusionValue);

        AkSoundEngine.SetRTPCValue(obstructionRTPCName, finalRTPCValue, gameObject);

        // 5. DEBUGGING (Colores para el Rayo)
        Color rayColor = Color.green; // 0
        if (occlusionValue > 0) rayColor = Color.red; // 100
        else if (obstructionValue > 0) rayColor = Color.yellow; // 50

        Debug.DrawRay(sourcePosition, directionToListener, rayColor);
    }

} // <--- ¡ESTE CORCHETE CIERRA LA CLASE Y EVITA EL ERROR CS1513!