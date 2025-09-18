using AK.Wwise;
using HurricaneVR.Framework.Components;
using UnityEngine;

[RequireComponent(typeof(HVRRotationTracker))]
public class HVR_WwisePhysicsDoor : HVRPhysicsDoor
{
    [Header("Wwise Events")]
    public AK.Wwise.Event doorOpenedWwiseEvent;
    public AK.Wwise.Event doorClosedWwiseEvent;
    public AK.Wwise.Event doorLockedWwiseEvent;
    public AK.Wwise.Event doorUnlockedWwiseEvent;

    [Header("Sonido de movimiento de la puerta")]
    [Tooltip("Evento de Wwise para el sonido continuo de la puerta chirriando.")]
    public AK.Wwise.Event doorSqueakWwiseEvent;

    [Tooltip("Evento para detener el sonido de chirrido.")]
    public AK.Wwise.Event doorSqueakStopWwiseEvent;

    [Tooltip("Umbral de velocidad de rotación (en grados por segundo) para activar el sonido de chirrido.")]
    public float squeakSpeedThreshold = 5.0f;

    private HVRRotationTracker rotationTracker;
    private bool isSqueakPlaying = false;

    // ELIMINAR 'override'. Usar el método Awake() normal.
    protected void Awake()
    {
        // Llamada a la clase base no es necesaria aquí porque Awake() no es virtual,
        // pero por convención y para evitar problemas con futuras actualizaciones
        // de HurricaneVR, se puede mantener.

        // base.Awake(); 

        rotationTracker = GetComponent<HVRRotationTracker>();

        if (rotationTracker == null)
        {
            Debug.LogError("El componente HVRRotationTracker no se encuentra en el objeto de la puerta.");
        }
    }

    protected override void Update()
    {
        base.Update();

        if (rotationTracker == null) return;

        // CORRECCIÓN: Usar 'rotationTracker.Angle' que es la variable pública.
        float angularVelocity = rotationTracker.Angle / Time.deltaTime;

        // Si la puerta se está moviendo lo suficientemente rápido y el sonido no está activo, actívalo.
        if (Mathf.Abs(angularVelocity) > squeakSpeedThreshold && !isSqueakPlaying)
        {
            if (doorSqueakWwiseEvent != null)
            {
                doorSqueakWwiseEvent.Post(gameObject);
                isSqueakPlaying = true;
            }
        }
        // Si la puerta se está moviendo muy lento o se detuvo y el sonido está activo, deténlo.
        else if (Mathf.Abs(angularVelocity) <= squeakSpeedThreshold && isSqueakPlaying)
        {
            if (doorSqueakStopWwiseEvent != null)
            {
                doorSqueakStopWwiseEvent.Post(gameObject);
                isSqueakPlaying = false;
            }
        }
    }

    protected override void OnDoorOpened()
    {
        base.OnDoorOpened();
        if (doorOpenedWwiseEvent != null)
        {
            doorOpenedWwiseEvent.Post(gameObject);
        }
    }

    protected override void OnDoorClosed()
    {
        base.OnDoorClosed();
        if (doorClosedWwiseEvent != null)
        {
            doorClosedWwiseEvent.Post(gameObject);
        }
    }

    public override void Lock()
    {
        base.Lock();
        if (doorLockedWwiseEvent != null)
        {
            doorLockedWwiseEvent.Post(gameObject);
        }
    }

    public override void Unlock()
    {
        base.Unlock();
        if (doorUnlockedWwiseEvent != null)
        {
            doorUnlockedWwiseEvent.Post(gameObject);
        }
    }
}