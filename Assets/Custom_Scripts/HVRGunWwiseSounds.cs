using AK.Wwise; // Necesario para Wwise Events
using HurricaneVR.Framework.Weapons; // Namespace de la clase padre HVRGunSounds
using UnityEngine;

// 1. Nos aseguramos de estar en el mismo namespace que la clase padre.
namespace HurricaneVR.Framework.Weapons
{
    // 2. Heredamos de la clase original de Unity AudioClip: HVRGunSounds.
    // Esto asegura que HVRGunBase.cs encuentre un componente compatible.
    public class HVRGunWwiseSounds : HVRGunSounds
    {
        [Header("Wwise Events")]
        [Tooltip("Wwise Event posted when the gun is fired.")]
        public AK.Wwise.Event FiredWwiseEvent;

        [Tooltip("Wwise Event posted when the gun is out of ammo.")]
        public AK.Wwise.Event OutOfAmmoWwiseEvent;

        [Tooltip("Wwise Event posted when the slide is pulled back.")]
        public AK.Wwise.Event SlideBackWwiseEvent;

        [Tooltip("Wwise Event posted when the slide moves forward.")]
        public AK.Wwise.Event SlideForwardWwiseEvent;

        // (Descomentar si usas la lógica de inserción/liberación de munición)
        // public AK.Wwise.Event AmmoInsertedWwiseEvent;
        // public AK.Wwise.Event AmmoReleasedWwiseEvent;


        // 3. Sobreescribimos (override) los métodos virtuales de la clase padre
        // para reemplazar la llamada a Unity AudioClip por Wwise Event.

        public override void PlayGunFire()
        {
            if (FiredWwiseEvent != null)
            {
                // Post el evento al mismo objeto de juego
                FiredWwiseEvent.Post(gameObject);
            }
            // No hacemos la llamada base.PlayGunFire(), anulando el sonido de Unity
        }

        public override void PlayOutOfAmmo()
        {
            if (OutOfAmmoWwiseEvent != null)
            {
                OutOfAmmoWwiseEvent.Post(gameObject);
            }
        }

        public override void PlaySlideForward()
        {
            if (SlideForwardWwiseEvent != null)
            {
                SlideForwardWwiseEvent.Post(gameObject);
            }
        }

        public override void PlaySlideBack()
        {
            if (SlideBackWwiseEvent != null)
            {
                SlideBackWwiseEvent.Post(gameObject);
            }
        }

        /*
        // Si la clase base hace virtuales estos métodos:
        public override void PlayAmmoInserted()
        {
            if (AmmoInsertedWwiseEvent != null)
            {
                AmmoInsertedWwiseEvent.Post(gameObject);
            }
        }

        public override void PlayAmmoReleased()
        {
            if (AmmoReleasedWwiseEvent != null)
            {
                AmmoReleasedWwiseEvent.Post(gameObject);
            }
        }
        */
    }
}
