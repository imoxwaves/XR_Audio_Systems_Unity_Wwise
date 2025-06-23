using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class VRHeightAdjuster : MonoBehaviour
{
    public float minHeight = 0.8f;  // Altura mínima (sentado)
    public float maxHeight = 2.0f;  // Altura máxima (de pie)
    public float heightSmoothTime = 0.2f; // Suavizado

    private CapsuleCollider capsule;
    private Rigidbody rb;
    private float targetHeight;
    private float heightVelocity;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        targetHeight = maxHeight; // Altura inicial
    }

    void Update()
    {
        // 1. Obtener altura real del headset
        float currentHeadHeight = transform.InverseTransformPoint(
            Camera.main.transform.position).y;

        // 2. Calcular altura objetivo (con límites)
        targetHeight = Mathf.Clamp(currentHeadHeight + 0.1f, minHeight, maxHeight);

        // 3. Suavizar transición
        capsule.height = Mathf.SmoothDamp(
            capsule.height,
            targetHeight,
            ref heightVelocity,
            heightSmoothTime);

        // 4. Ajustar centro del collider
        capsule.center = new Vector3(0, capsule.height / 2f, 0);

        // 5. Corregir posición para evitar clipping
        if (rb.velocity.y == 0) // Solo si no está en movimiento
        {
            float heightDifference = targetHeight - capsule.height;
            rb.MovePosition(transform.position + Vector3.up * heightDifference);
        }
    }
}