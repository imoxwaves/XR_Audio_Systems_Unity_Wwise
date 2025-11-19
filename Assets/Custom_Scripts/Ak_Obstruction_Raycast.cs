using UnityEngine;
// TSD Script: Implements dynamic sound occlusion and obstruction using dual Raycasting.
// This is critical for realistic spatial audio filtering in large 3D environments.
public class AkObstruction_Raycast : MonoBehaviour
{
    // --- PUBLIC VARIABLES (Exposed for Designer Tuning in Inspector) ---
    [Header("Wwise & System Settings")]
    [Tooltip("The Wwise Game Parameter (RTPC) used to control filtering/volume.")]
    public string obstructionRTPCName = "ObstructionAmount";

    [Tooltip("Maximum distance for Raycast detection. Beyond this, audio is treated as Clear (0).")]
    public float maxDistance = 30f;

    [Tooltip("Layers that will obstruct or occlude the sound (e.g., Walls, Doors).")]
    public LayerMask obstructionMask;

    [Header("Performance Optimization")]
    [Tooltip("Frequency (in seconds) to run the Raycast logic. Lower value = more CPU use.")]
    public float updateFrequency = 0.2f;

    // --- PRIVATE VARIABLES (Internal Logic) ---
    private Transform audioListener;
    private float nextUpdateTime;

    void Start()
    {
        // Cache the listener's transform (typically the Main Camera/Player Head in VR).
        audioListener = Camera.main.transform;
    }

    void Update()
    {
        // PERFORMANCE THREOTTLE: Only runs the expensive Physics.Raycast check at the defined frequency.
        if (Time.time >= nextUpdateTime)
        {
            CalculateObstruction();
            nextUpdateTime = Time.time + updateFrequency;
        }
    }

    /// <summary>
    /// Uses a dual Raycast strategy to determine one of three states: Clear (0), Obstructed (50), or Occluded (100).
    /// Sends the corresponding value to the designated Wwise RTPC.
    /// </summary>
    void CalculateObstruction()
    {
        // 1. DEFINE VECTORS
        Vector3 sourcePosition = transform.position;
        Vector3 listenerPosition = audioListener.position;
        Vector3 directionToListener = listenerPosition - sourcePosition;
        float distanceToListener = directionToListener.magnitude;

        // Exit early if the listener is outside the detection range to save CPU.
        if (distanceToListener > maxDistance)
        {
            AkSoundEngine.SetRTPCValue(obstructionRTPCName, 0f, gameObject);
            return;
        }

        // Initialize state variables (RTPC values for Wwise).
        float obstructionRTPCValue = 0f; // 0 = Clear
        float occlusionRTPCValue = 0f; // 0 = Clear

        RaycastHit hit;

        // 2. FIRST RAYCAST: OBSTRUCTION CHECK (Source -> Listener)
        // Detects any blockage between the sound emitter and the player.
        if (Physics.Raycast(sourcePosition, directionToListener, out hit, distanceToListener, obstructionMask))
        {
            // If the ray hits, the sound is at least Obstructed (partial filtering).
            obstructionRTPCValue = 50f;
        }

        // 3. SECOND RAYCAST: OCCLUSION CHECK (Listener -> Source)
        // Only check for full Occlusion if Obstrucion was already detected to save CPU cycles.
        if (obstructionRTPCValue > 0f)
        {
            Vector3 directionToSource = sourcePosition - listenerPosition;

            // Note: The second Raycast checks the inverse direction, ensuring line-of-sight is blocked both ways.
            if (Physics.Raycast(listenerPosition, directionToSource, out hit, distanceToListener, obstructionMask))
            {
                // If the ray hits from both ends, it's considered FULL OCCLUSION.
                occlusionRTPCValue = 100f;
            }
        }

        // 4. FINAL DECISION AND WWISE SEND
        // Prioritize the highest value: 100 (Occlusion) > 50 (Obstruction) > 0 (Clear).
        float finalRTPCValue = Mathf.Max(obstructionRTPCValue, occlusionRTPCValue);

        // Sends the final RTPC value to the Wwise Game Parameter, tied to the emitter GameObject.
        AkSoundEngine.SetRTPCValue(obstructionRTPCName, finalRTPCValue, gameObject);

        // 5. DEBUGGING: Visualizing the Raycast in the Editor (TSD best practice)
        Color rayColor = Color.green; // Clear
        if (occlusionRTPCValue > 0) rayColor = Color.red; // Occlusion
        else if (obstructionRTPCValue > 0) rayColor = Color.yellow; // Obstruction

        Debug.DrawRay(sourcePosition, directionToListener, rayColor);
    }
}