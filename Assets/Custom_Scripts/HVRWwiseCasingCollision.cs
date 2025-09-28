using UnityEngine;
using AK.Wwise; // Necesario para eventos de Wwise

// Este script DEBE ser añadido al PREFAB del casquillo de bala.
public class HVRWwiseCasingCollision : MonoBehaviour
{
    [Tooltip("Wwise Event que se postea cuando el casquillo golpea una superficie.")]
    public AK.Wwise.Event CollisionWwiseEvent;

    [Tooltip("Velocidad mínima de colisión para que se dispare el sonido.")]
    [Range(0f, 5f)]
    public float MinCollisionVelocity = 0.5f;

    [Tooltip("Duración en segundos antes de destruir el casquillo (limpieza de escena).")]
    public float Lifetime = 5f;

    private void Start()
    {
        // Limpiamos el objeto después de un tiempo para evitar sobrecargar la escena
        Destroy(gameObject, Lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificamos si la colisión fue lo suficientemente fuerte
        if (collision.relativeVelocity.magnitude > MinCollisionVelocity)
        {
            // Opcional: Podrías usar un AK.Wwise.Switch para diferenciar el sonido
            // basado en el material del objeto golpeado (collision.collider.tag o Layer).

            if (CollisionWwiseEvent != null)
            {
                // Post el evento al objeto del casquillo
                CollisionWwiseEvent.Post(gameObject);
            }

            // Opcional: Desactivar este componente después del primer impacto
            // para evitar que suene en cada rebote, si solo quieres el primer golpe fuerte.
            // enabled = false; 
        }
    }
}
