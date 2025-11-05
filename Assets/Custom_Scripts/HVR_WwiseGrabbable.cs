using AK.Wwise; // Necesario para Wwise
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using UnityEngine;

// Este script extiende la funcionalidad de la clase base HVRGrabbable.
// NOTA IMPORTANTE: Este script debe ser adjuntado al MISMO GameObject
// que ya tiene el componente HVRGrabbable.
public class HVRWwiseGrabbable : MonoBehaviour
{
    [Header("Wwise Events")]
    [Tooltip("Wwise Event to post when the object is grabbed by a hand.")]
    public AK.Wwise.Event handGrabbedWwiseEvent;

    [Tooltip("Wwise Event to post when the object hits a surface.")]
    public AK.Wwise.Event collisionWwiseEvent;

    private HVRGrabbable _grabbable;

    private void Start()
    {
        // Obtener la referencia al componente HVRGrabbable en el mismo objeto.
        _grabbable = GetComponent<HVRGrabbable>();
        if (_grabbable == null)
        {
            Debug.LogError("HVRWwiseGrabbable requiere un componente HVRGrabbable en el mismo GameObject.");
            return;
        }

        // Suscribirse al evento de la clase base para saber cuándo se ha agarrado con la mano.
        _grabbable.HandGrabbed.AddListener(OnHandGrabbedEvent);
    }

    private void OnDestroy()
    {
        // Es crucial remover el listener para evitar errores cuando el objeto se destruye.
        if (_grabbable != null && _grabbable.HandGrabbed != null)
        {
            _grabbable.HandGrabbed.RemoveListener(OnHandGrabbedEvent);
        }
    }

    // Este es el método que se llama cuando el evento HandGrabbed se dispara.
    private void OnHandGrabbedEvent(HVRHandGrabber grabber, HVRGrabbable grabbable)
    {
        // Postear el evento de Wwise.
        if (handGrabbedWwiseEvent != null)
        {
            handGrabbedWwiseEvent.Post(gameObject);
        }
    }

    // Este método se activa cuando el objeto colisiona con otro.
    private void OnCollisionEnter(Collision collision)
    {
        // Evita que el sonido se active constantemente por pequeños rebotes.
        // La magnitud de la velocidad del objeto es una buena medida.
        if (collisionWwiseEvent != null && collision.relativeVelocity.magnitude > 2.0f)
        {
            collisionWwiseEvent.Post(gameObject);
        }
    }
}