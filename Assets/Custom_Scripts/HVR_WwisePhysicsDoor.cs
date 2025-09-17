using UnityEngine;
using AK.Wwise;
using HurricaneVR.Framework.Components;

// Este script hereda directamente de HVRPhysicsDoor.
// Esto nos permite sobrescribir (override) sus métodos protegidos.
public class HVR_WwisePhysicsDoor : HVRPhysicsDoor
{
    [Header("Eventos de Wwise para la Puerta")]
    public AK.Wwise.Event doorOpenedWwiseEvent;
    public AK.Wwise.Event doorClosedWwiseEvent;

    // Sobrescribimos el método OnDoorOpened para añadir nuestra funcionalidad de audio.
    // 'protected virtual void OnDoorOpened()' es la firma del método en la clase base.
    protected override void OnDoorOpened()
    {
        // Primero, llamamos a la implementación original del método en la clase base.
        // Esto asegura que la lógica original de HVR (como los Debug.Log) siga funcionando.
        base.OnDoorOpened();

        // Luego, disparamos nuestro evento de Wwise.
        if (doorOpenedWwiseEvent != null)
        {
            doorOpenedWwiseEvent.Post(gameObject);
        }
    }

    // Sobrescribimos el método OnDoorClosed de la misma manera.
    protected override void OnDoorClosed()
    {
        base.OnDoorClosed();

        if (doorClosedWwiseEvent != null)
        {
            doorClosedWwiseEvent.Post(gameObject);
        }
    }
}