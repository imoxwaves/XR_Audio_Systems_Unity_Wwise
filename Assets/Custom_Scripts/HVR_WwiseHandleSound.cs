using AK.Wwise;
using HurricaneVR.Framework.Components;
using UnityEngine;

public class HVR_WwiseHandleSound : MonoBehaviour
{
    [Header("Referencias de HVR")]
    [Tooltip("Arrastra el HVRRotationTracker del objeto de la manija aquí.")]
    public HVRRotationTracker rotationTracker;

    [Header("Eventos de Wwise")]
    [Tooltip("El evento de Wwise para el sonido de la manija (debe ser un Random Container).")]
    public AK.Wwise.Event handleTurnWwiseEvent;

    [Tooltip("El evento de Wwise para detener el sonido de la manija.")]
    public AK.Wwise.Event handleStopWwiseEvent;

    [Tooltip("Umbral de ángulo mínimo para que el sonido se active. Elige un valor pequeño (e.g., 0.1).")]
    public float angleThreshold = 0.1f;

    private bool isSoundPlaying = false;
    private float lastAngle;

    void Start()
    {
        if (rotationTracker == null)
        {
            Debug.LogError("El HVRRotationTracker no está asignado.");
            return;
        }

        // Suscribirse al evento que se dispara en cada cambio de ángulo
        rotationTracker.AngleChanged.AddListener(OnAngleChanged);

        // Obtener el ángulo inicial
        lastAngle = rotationTracker.UnsignedAngle;
    }

    private void OnAngleChanged(float angle, float delta)
    {
        if (handleTurnWwiseEvent == null) return;

        // Si el cambio de ángulo es significativo y el sonido no está reproduciéndose, inícialo.
        if (Mathf.Abs(delta) > angleThreshold && !isSoundPlaying)
        {
            handleTurnWwiseEvent.Post(gameObject);
            isSoundPlaying = true;
        }
    }

    void Update()
    {
        if (rotationTracker == null) return;

        // Comprueba si el ángulo ha dejado de cambiar.
        float currentAngle = rotationTracker.UnsignedAngle;
        bool hasStoppedMoving = Mathf.Abs(currentAngle - lastAngle) <= angleThreshold;

        // Si la manija se ha detenido y el sonido está reproduciéndose, deténlo.
        if (hasStoppedMoving && isSoundPlaying)
        {
            if (handleStopWwiseEvent != null)
            {
                handleStopWwiseEvent.Post(gameObject);
            }
            else
            {
                // Si no hay un evento de "stop" específico, usa el método de Wwise para detenerlo.
                // handleTurnWwiseEvent.Stop(gameObject); // Esta es una forma más avanzada si lo configuras en Wwise
            }

            isSoundPlaying = false;
        }

        lastAngle = currentAngle;
    }

    void OnDisable()
    {
        if (rotationTracker != null)
        {
            rotationTracker.AngleChanged.RemoveListener(OnAngleChanged);
        }
    }
}