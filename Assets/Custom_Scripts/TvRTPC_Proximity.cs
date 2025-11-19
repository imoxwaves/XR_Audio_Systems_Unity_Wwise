using UnityEngine;

// TSD Script: Controls an AkRTPC (Wwise Game Parameter) based on player proximity.
// Purpose: To drive audio modulation (pitch, distortion, LPF) for the 'corruption' effect.
public class TvRTPC_Proximity : MonoBehaviour
{
    // --- PUBLIC VARIABLES (Exposed in the Unity Inspector) ---
    [Header("Wwise RTPC Settings")]
    [Tooltip("The name of the Wwise Game Parameter (RTPC) to control. E.g., 'CorruptionDistance'")]
    public string rtpcName = "CorruptionDistance";

    [Tooltip("The maximum distance for the RTPC range (0-100). Should match the attenuation or trigger range.")]
    public float maxRTPCDistance = 15f;

    [Header("Performance Optimization")]
    [Tooltip("Frequency (in seconds) to check distance. 0.2f = 5 checks per second.")]
    public float updateFrequency = 0.2f;

    // --- PRIVATE VARIABLES (Internal Logic) ---
    private Transform playerCamera;
    private float nextUpdateTime;

    void Start()
    {
        // Cache the main camera transform, which serves as the player's head (listener) in VR.
        playerCamera = Camera.main.transform;

        // Ensure initial RTPC value is set to avoid audio glitches on scene load.
        CalculateAndSendRTPC();
    }

    void Update()
    {
        // PERFORMANCE OPTIMIZATION: Implements throttling to save CPU cycles, critical for VR.
        if (Time.time >= nextUpdateTime)
        {
            CalculateAndSendRTPC();
            nextUpdateTime = Time.time + updateFrequency;
        }
    }

    void CalculateAndSendRTPC()
    {
        // 1. Calculate the raw distance between the sound source (this GameObject) and the player's head.
        float distance = Vector3.Distance(transform.position, playerCamera.position);

        // 2. Normalize the distance (converts range 0 to maxRTPCDistance into a 0.0 to 1.0 value).
        // The closer the player is (distance=0), the lower the normalized value.
        // The further the player is (distance=Max), the higher the normalized value (approaching 1.0).
        float distanceNormalized = Mathf.Clamp01(distance / maxRTPCDistance);

        // 3. Scale to Wwise range (0-100) and send to Wwise API.
        // RTPC range is intentionally set to max 15. Standard Wwise is 0-100.
        float rtpcValue = distanceNormalized * 15f;

        // Send the final RTPC value to the Wwise Game Parameter, tied to this GameObject.
        AkSoundEngine.SetRTPCValue(rtpcName, rtpcValue, gameObject);
    }
}
