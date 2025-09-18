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
    private float lastClampedAngle;

    // Se elimina la llamada a base.Awake() para evitar el error.
    protected void Awake()
    {
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

        float currentClampedAngle = rotationTracker.ClampedAngle;
        float angleDelta = Mathf.Abs(currentClampedAngle - lastClampedAngle);
        float angularVelocity = angleDelta / Time.deltaTime;

        if (angularVelocity > squeakSpeedThreshold && !isSqueakPlaying)
        {
            if (doorSqueakWwiseEvent != null)
            {
                doorSqueakWwiseEvent.Post(gameObject);
                isSqueakPlaying = true;
            }
        }
        else if (angularVelocity <= squeakSpeedThreshold && isSqueakPlaying)
        {
            if (doorSqueakStopWwiseEvent != null)
            {
                doorSqueakStopWwiseEvent.Post(gameObject);
                isSqueakPlaying = false;
            }
        }

        lastClampedAngle = currentClampedAngle;
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