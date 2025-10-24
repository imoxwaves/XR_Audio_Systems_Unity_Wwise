using UnityEngine;

public class TvRTPC_Proximity : MonoBehaviour
{
    // VARIABLES PÚBLICAS
    // ---
    public string rtpcName = "CorruptionDistance"; // Nombre del RTPC de Wwise
    public float maxRTPCDistance = 15f;          // Rango máximo del RTPC (debe coincidir con tu Trigger)
    public float updateFrequency = 0.2f;         // Optimización: Chequear 5 veces por segundo

    // VARIABLES PRIVADAS
    // ---
    private Transform playerCamera;
    private float nextUpdateTime;

    void Start()
    {
        // Esto asume que la cámara principal es la "cabeza" del jugador VR
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // Optimización: Solo calcula si ha pasado el tiempo necesario
        if (Time.time >= nextUpdateTime)
        {
            CalculateAndSendRTPC();
            nextUpdateTime = Time.time + updateFrequency;
        }
    }

    void CalculateAndSendRTPC()
    {
        float distance = Vector3.Distance(transform.position, playerCamera.position);

        // *******************************************************************
    // LÍNEA MODIFICADA PARA INVERTIR LA LÓGICA:
    // Ya NO restamos 1.0f - (...). Esto hace que:
    // Cerca (distance=0)      -> distanceNormalized=0   -> RTPC=0 (NORMAL)
    // Lejos (distance=Max)   -> distanceNormalized=1.0 -> RTPC=100 (CORRUPCIÓN)
    // *******************************************************************
        // Clamp01 asegura que el valor esté entre 0 y 1, independientemente de la distancia
        float distanceNormalized = Mathf.Clamp01(distance / maxRTPCDistance);

        // Envía el valor al RTPC (0-100) en el GameObject del TV
        AkSoundEngine.SetRTPCValue(rtpcName, distanceNormalized * 15f, gameObject);
    }
}
