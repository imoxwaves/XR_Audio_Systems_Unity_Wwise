using UnityEngine;
using AK.Wwise;

// Requiere el componente CharacterController para calcular la velocidad.
[RequireComponent(typeof(CharacterController))]
public class VRPlayerFootsteps : MonoBehaviour
{
    //Wwise Events
    [Header("Wwise Events")]
    public AK.Wwise.Event myFootstep;
    public AK.Wwise.RTPC mySpeed;

    // Footsteps Settings
    [Header("Footsteps Settings")]
    [Tooltip("Intervalo entre pasos cuando se camina (velocidad normal)")]
    public float WalkStepInterval = 0.5f;
    [Tooltip("Intervalo entre pasos cuando se corre (sprint)")]
    public float RunStepInterval = 0.3f;
    [Tooltip("Velocidad mínima para considerar movimiento")]
    public float MinSpeedForFootsteps = 0.1f;

    // Player Grounded Check
    [Header("Player Grounded")]
    [Tooltip("Si el personaje está en el suelo o no.")]
    public bool Grounded = true;
    [Tooltip("Desplazamiento para la comprobación del suelo.")]
    public float GroundedOffset = -0.14f;
    [Tooltip("El radio de la comprobación del suelo.")]
    public float GroundedRadius = 0.5f;
    [Tooltip("Capas que el personaje considera como suelo.")]
    public LayerMask GroundLayers;

    // Internal variables
    private CharacterController _controller;
    private float lastFootstepTime = 0.0f;
    private float _speed = 0.0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        // Asegurarse de que el RTPC de velocidad comience en 0.
        mySpeed.SetGlobalValue(0);
    }

    void Update()
    {
        // 1. Verificar si el jugador está en el suelo.
        GroundedCheck();

        // 2. Calcular la velocidad de movimiento horizontal.
        Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z);
        _speed = horizontalVelocity.magnitude;

        // 3. Actualizar el valor del RTPC de Wwise con la velocidad.
        // Multiplicamos por un factor (por ejemplo, 25) para que el valor sea más significativo para Wwise.
        mySpeed.SetGlobalValue(25 * _speed);

        // 4. Lógica de los pasos.
        // Comprobar si el jugador está en el suelo y moviéndose lo suficientemente rápido.
        if (Grounded && _speed >= MinSpeedForFootsteps)
        {
            // Determinar el intervalo de pasos basado en la velocidad.
            // Usamos una interpolación simple entre el intervalo de caminar y correr.
            float currentStepInterval = Mathf.Lerp(WalkStepInterval, RunStepInterval, Mathf.InverseLerp(MinSpeedForFootsteps, 4.0f, _speed));

            // Si ha pasado suficiente tiempo desde el último paso, reproducir el sonido.
            if (Time.time - lastFootstepTime > currentStepInterval)
            {
                myFootstep.Post(gameObject);
                lastFootstepTime = Time.time;
            }
        }
    }

    private void GroundedCheck()
    {
        // Posición de la esfera de detección del suelo.
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        // Comprueba si la esfera colisiona con cualquier capa del suelo.
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
}
