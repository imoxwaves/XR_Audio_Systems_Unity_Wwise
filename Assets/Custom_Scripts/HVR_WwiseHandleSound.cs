using AK.Wwise; // Necesario para Wwise
using HurricaneVR.Framework.Components; // Necesario para HVRRotationTracker
using UnityEngine;

public class HVR_WwiseHandleSound : MonoBehaviour
{
    [Header("Referencias de HVR")]
    [Tooltip("Arrastra el HVRRotationTracker del objeto de la manija aquí.")]
    public HVRRotationTracker rotationTracker;

    [Header("Eventos de Wwise")]
    [Tooltip("Evento de Wwise para el sonido de girar la manija.")]
    public AK.Wwise.Event handleTurnWwiseEvent;

    private bool hasPlayedSound = false;

    void Start()
    {
        if (rotationTracker == null)
        {
            Debug.LogError("El HVRRotationTracker no está asignado. No se pueden reproducir sonidos.");
            return;
        }

        // Suscribirse al evento AngleChanged del rotationTracker
        // Esto es mucho más eficiente que usar el método Update()
        rotationTracker.AngleChanged.AddListener(OnAngleChanged);
    }

    private void OnAngleChanged(float angle, float delta)
    {
        // Solo reproducir el sonido si el evento de Wwise está asignado
        if (handleTurnWwiseEvent == null) return;

        // Reproducir el sonido la primera vez que la manija empieza a girar
        // Para que el sonido solo se reproduzca una vez por cada "giro"
        if (!hasPlayedSound)
        {
            handleTurnWwiseEvent.Post(gameObject);
            hasPlayedSound = true;
        }
    }

    // Unsubscribe from the event when the object is disabled or destroyed
    void OnDisable()
    {
        if (rotationTracker != null)
        {
            rotationTracker.AngleChanged.RemoveListener(OnAngleChanged);
        }
    }

    // Reset the sound flag when the handle stops turning for a moment
    private void OnHandleStopped()
    {
        hasPlayedSound = false;
    }
}