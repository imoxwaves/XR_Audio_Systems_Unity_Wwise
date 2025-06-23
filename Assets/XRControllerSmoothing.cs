// Este script debe adjuntarse al GameObject 'Left Controller' y 'Right Controller'.
// Suaviza la posición y rotación del controlador para reducir el jitter en objetos agarrados con Velocity Tracking.

using UnityEngine;

public class XRControllerSmoothing : MonoBehaviour
{
    // Velocidad de suavizado para la posición.
    // Cuanto mayor el valor, más rápido el suavizado (menos lag, menos suavizado).
    // Cuanto menor el valor (cercano a 0), más lento el suavizado (más lag, más suavizado).
    // Prueba valores entre 5.0 y 20.0.
    [Tooltip("Velocidad a la que el controlador suaviza su posición. Mayor = más rápido (menos suavizado).")]
    [SerializeField] private float positionSmoothingSpeed = 10.0f; // Valor inicial recomendado.

    // Velocidad de suavizado para la rotación.
    // Cuanto mayor el valor, más rápido el suavizado.
    // Prueba valores entre 5.0 y 20.0.
    [Tooltip("Velocidad a la que el controlador suaviza su rotación. Mayor = más rápido (menos suavizado).")]
    [SerializeField] private float rotationSmoothingSpeed = 10.0f; // Valor inicial recomendado.

    // Variables para almacenar la posición y rotación objetivo del controlador.
    // Estas son las posiciones/rotaciones "deseadas" que vienen del tracking de hardware.
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    /// <summary>
    /// Awake se llama cuando el script se carga.
    /// Inicializa las posiciones y rotaciones objetivo a la posición/rotación actual del controlador.
    /// </summary>
    private void Awake()
    {
        // Inicializamos las posiciones/rotaciones objetivo a la actual del controlador.
        // Esto evita un "salto" inicial cuando el script empieza a ejecutarse.
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    /// <summary>
    /// LateUpdate se llama una vez por frame, después de que todos los Updates y animaciones se han procesado.
    /// Esto es crucial: asegura que la posición y rotación "bruta" del tracking VR (aplicada por el Tracked Pose Driver
    /// o XR Controller) ya esté actualizada antes de que la suavicemos.
    /// </summary>
    private void LateUpdate()
    {
        // La posición y rotación actual del Transform ya ha sido actualizada por el Tracked Pose Driver.
        // Esta es nuestra "posición/rotación deseada" para este frame.
        targetPosition = transform.position;
        targetRotation = transform.rotation;

        // Suavizamos la posición actual del Transform del controlador.
        // Vector3.Lerp interpola linealmente entre dos puntos.
        // Time.deltaTime * positionSmoothingSpeed controla la velocidad del suavizado.
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionSmoothingSpeed);

        // Suavizamos la rotación actual del Transform del controlador.
        // Quaternion.Slerp interpola esféricamente entre dos rotaciones, lo cual es ideal para rotaciones suaves.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothingSpeed);
    }
}

