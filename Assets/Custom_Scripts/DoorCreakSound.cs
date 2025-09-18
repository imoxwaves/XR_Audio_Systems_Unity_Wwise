using UnityEngine;
using AK.Wwise;

public class DoorCreakSound : MonoBehaviour
{
    // Una variable pública para asignar el HingeJoint de la puerta desde el Inspector de Unity.
    // Usamos el nombre "_doorHinge" para evitar conflictos con la propiedad "hingeJoint" de Unity.
    public HingeJoint _doorHinge;

    // Una variable pública para asignar el evento de sonido de Wwise.
    // Esto te permite elegir qué sonido se reproduce.
    public AK.Wwise.Event creakEvent;

    // Un umbral de velocidad. El sonido solo se activará si el movimiento 
    // de la puerta (su velocidad angular) supera este valor.
    public float creakThreshold = 0.5f;

    // Un factor para cambiar el tono del sonido (pitch) en función de la velocidad. 
    // Si la puerta se mueve más rápido, el sonido será más agudo.
    public float pitchFactor = 0.1f;

    // Una bandera booleana para saber si el sonido ya está sonando. 
    // Esto evita que se reproduzca repetidamente cada frame.
    private bool isCreaking = false;

    // El método 'Update' se llama en cada frame del juego.
    // Es donde comprobamos continuamente la condición de la puerta.
    void Update()
    {
        // Obtiene la velocidad angular del HingeJoint.
        // El valor es un float que indica la velocidad de rotación.
        float angularVelocity = _doorHinge.velocity;

        // Comprobamos si el valor absoluto de la velocidad es mayor que nuestro umbral.
        // Usamos Mathf.Abs() para ignorar la dirección del movimiento (abrir o cerrar).
        if (Mathf.Abs(angularVelocity) > creakThreshold)
        {
            // Si la puerta se mueve lo suficientemente rápido y el sonido no está activo...
            if (!isCreaking)
            {
                // ...llamamos al evento de Wwise para empezar a reproducir el sonido.
                creakEvent.Post(gameObject);

                // Y actualizamos nuestra bandera para saber que ya está sonando.
                isCreaking = true;
            }

            // Opcional: Calcula y aplica un nuevo tono (pitch) al sonido.
            // A mayor velocidad, mayor será el pitch.
            float currentPitch = 1.0f + Mathf.Abs(angularVelocity) * pitchFactor;

            // Envía el nuevo valor del pitch a Wwise usando un RTPC (Real-Time Parameter Control).
            AkSoundEngine.SetRTPCValue("CreakPitch", currentPitch, gameObject);
        }
        else // Si la velocidad de la puerta es igual o menor al umbral...
        {
            // ...y el sonido estaba activo...
            if (isCreaking)
            {
                // ...lo detenemos.
                creakEvent.Stop(gameObject);

                // Y actualizamos la bandera para reflejar que se ha detenido.
                isCreaking = false;
            }
        }
    }
}