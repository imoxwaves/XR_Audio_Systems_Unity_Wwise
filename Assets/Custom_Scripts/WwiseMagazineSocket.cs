using UnityEngine;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Weapons.Guns;

// Asegúrate de incluir el namespace de Wwise para poder usar AkSoundEngine
// Si usas el componente Wwise Unity Integration, es probable que no necesites un 'using' explícito aquí,
// pero AkSoundEngine es la forma más robusta de garantizar la compatibilidad.
// Para este ejemplo, asumiremos que estás usando el componente AkEvent o que AkSoundEngine es accesible.

namespace YourGameNamespace // Reemplaza esto con el namespace de tu juego o déjalo vacío si no usas uno
{
    // Este script hereda de HVRMagazineSocket, dándonos acceso a su lógica interna.
    public class WwiseMagazineSocket : HVRMagazineSocket
    {
        [Header("Wwise Events")]
        [Tooltip("Evento de Wwise a disparar cuando el cargador es INSERTADO con éxito.")]
        public AK.Wwise.Event MagazineInsertedEvent;

        [Tooltip("Evento de Wwise a disparar cuando el cargador es EXPULSADO/SOLTADO.")]
        public AK.Wwise.Event MagazineEjectedEvent;

        /// <summary>
        /// Sobrescribimos el método original para insertar el cargador.
        /// </summary>
        protected override void OnGrabbableParented(HVRGrabbable grabbable)
        {
            // 1. Primero, llamamos a la implementación original del script base (HVRMagazineSocket).
            // Esto asegura que toda la física y la animación de HurricaneVR ocurran primero.
            base.OnGrabbableParented(grabbable);

            // 2. Después de que la lógica base se ejecuta, disparamos el evento de Wwise.
            if (MagazineInsertedEvent != null)
            {
                // Dispara el evento en el objeto del cargador o en el objeto socket, dependiendo de tu diseño de audio.
                // Usaremos el socket (este GameObject) como la fuente del audio.
                MagazineInsertedEvent.Post(gameObject);
            }
        }

        /// <summary>
        /// Sobrescribimos el método original llamado cuando el cargador es soltado/expulsado.
        /// </summary>
        protected override void OnReleased(HVRGrabbable grabbable)
        {
            // 1. Primero, llamamos a la implementación original del script base (HVRMagazineSocket).
            // Esto maneja la eyección animada y la física de HurricaneVR.
            base.OnReleased(grabbable);

            // 2. Después de que la lógica base se ejecuta, disparamos el evento de Wwise.
            if (MagazineEjectedEvent != null)
            {
                // Dispara el evento en el objeto del cargador o en el objeto socket.
                MagazineEjectedEvent.Post(gameObject);
            }
        }
    }
}
