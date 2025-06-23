// Este script debe adjuntarse al GameObject 'XR Origin (XR Rig)'.
// Su propósito es aplicar una fuerza de gravedad constante al CharacterController
// del jugador, evitando que flote o se desplace hacia arriba.

using UnityEngine;

public class XRPlayerGravity : MonoBehaviour
{
    // [SerializeField] permite ver y modificar estas variables en el Inspector de Unity,
    // pero las mantiene privadas para otras clases.

    // Valor de la gravedad (negativo para que tire hacia abajo).
    // El valor predeterminado de la gravedad en Unity es aproximadamente -9.81 m/s².
    [SerializeField] private float gravity = -9.81f;

    // Pequeña fuerza descendente aplicada cuando el jugador ya está en el suelo.
    // Esto ayuda a mantener al jugador firmemente "pegado" al suelo,
    // incluso en superficies ligeramente irregulares o pendientes,
    // evitando pequeños rebotes o deslizamientos.
    [SerializeField] private float groundedGravity = -0.5f;

    // Referencia al componente CharacterController del jugador.
    // Este script moverá el CharacterController para aplicar la gravedad.
    [SerializeField] private CharacterController characterController;

    // Almacena la velocidad vertical actual del jugador.
    // Se usa para acumular la fuerza de la gravedad.
    private Vector3 verticalVelocity;

    /// <summary>
    /// Awake se llama cuando el script se carga.
    /// Se usa para inicializar referencias.
    /// </summary>
    private void Awake()
    {
        // Intentar obtener el CharacterController en el mismo GameObject.
        // Si ya está asignado en el Inspector, esta línea no tendrá efecto.
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
            // Si no se encuentra el CharacterController, mostrar un error y desactivar el script.
            if (characterController == null)
            {
                UnityEngine.Debug.LogError("XRPlayerGravity requiere un CharacterController en el mismo GameObject o asignado en el Inspector.");
                enabled = false; // Desactiva este script para evitar errores.
            }
        }
    }

    /// <summary>
    /// Update se llama una vez por cada frame.
    /// Aquí se calcula y aplica la gravedad.
    /// </summary>
    private void Update()
    {
        // Comprobar si el CharacterController está actualmente en el suelo.
        // 'isGrounded' es una propiedad del CharacterController que detecta si está tocando el suelo.
        if (characterController.isGrounded)
        {
            // Si está en el suelo, establecer una pequeña velocidad vertical descendente.
            // Esto evita que la velocidad vertical se acumule indefinidamente
            // y asegura que el jugador se mantenga firmemente en el suelo.
            verticalVelocity.y = groundedGravity;
        }
        else
        {
            // Si no está en el suelo (está en el aire), aplicar la aceleración de la gravedad.
            // Se multiplica por Time.deltaTime para que sea independiente de la velocidad de los fotogramas.
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        // Aplicar la velocidad vertical al CharacterController.
        // El método CharacterController.Move() mueve el CharacterController y,
        // al mismo tiempo, maneja las colisiones durante el movimiento.
        // Los Locomotion Providers (como Continuous Move Provider) se encargarán del movimiento horizontal.
        // Aquí solo nos ocupamos del movimiento vertical.
        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}
