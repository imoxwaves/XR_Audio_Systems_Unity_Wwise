using UnityEngine;
using AK.Wwise;

// TSD Script: Manages player footsteps, speed-based cadence, and ground detection.
// NOTE: This approach uses timing/interval and speed calculation (timing-based) rather than Animation Events.
[RequireComponent(typeof(CharacterController))]
public class VRPlayerFootsteps : MonoBehaviour
{
    // --- WWISE INTEGRATION ---
    [Header("Wwise Integration")]
    [Tooltip("Wwise Event posted when a footstep should be heard.")]
    public AK.Wwise.Event myFootstep;

    [Tooltip("Wwise Game Parameter (RTPC) that controls the global player speed for effects.")]
    public AK.Wwise.RTPC mySpeed;

    // --- CADENCE & SPEED SETTINGS ---
    [Header("Footsteps Cadence")]
    [Tooltip("Time interval (seconds) between steps when walking (low speed).")]
    public float WalkStepInterval = 0.5f;

    [Tooltip("Time interval (seconds) between steps when sprinting (high speed).")]
    public float RunStepInterval = 0.3f;

    [Tooltip("Minimum horizontal velocity required to trigger footstep sounds.")]
    public float MinSpeedForFootsteps = 0.1f;

    // --- GROUND CHECK (Physics-Based Surface Detection) ---
    [Header("Player Grounded Check")]
    [Tooltip("Set to true if the character is currently touching the ground.")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.5f;

    [Tooltip("Physics Layers that the character considers as solid ground.")]
    public LayerMask GroundLayers;

    // --- INTERNAL LOGIC VARIABLES ---
    private CharacterController _controller;
    private float lastFootstepTime = 0.0f;
    private float _speed = 0.0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        // Ensure the global Wwise speed RTPC starts at 0 to prevent audio glitches.
        mySpeed.SetGlobalValue(0);
    }

    void Update()
    {
        // 1. Physics Check: Determine if the player is touching the ground.
        GroundedCheck();

        // 2. Speed Calculation: Get the horizontal velocity magnitude from the CharacterController.
        Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z);
        _speed = horizontalVelocity.magnitude;

        // 3. Wwise RTPC Update: Send the current speed (scaled) to a global RTPC for dynamic effects.
        // The scale factor (25) should match the Wwise RTPC's expected range (if 0-100).
        mySpeed.SetGlobalValue(25 * _speed);

        // 4. Footstep Timing Logic: Only proceed if moving and grounded.
        if (Grounded && _speed >= MinSpeedForFootsteps)
        {
            // ADVANCED CADENCE: Use Mathf.Lerp to smoothly interpolate the step interval based on current speed.
            // This prevents sudden jumps between 'walking' and 'running' cadence.
            // NOTE: The '4.0f' represents the max speed expected for running.
            float currentStepInterval = Mathf.Lerp(WalkStepInterval, RunStepInterval, Mathf.InverseLerp(MinSpeedForFootsteps, 4.0f, _speed));

            // 5. Post Event: Check if enough time has passed since the last footstep.
            if (Time.time - lastFootstepTime > currentStepInterval)
            {
                // TODO: A Raycast here would determine the surface material before Post()
                myFootstep.Post(gameObject);
                lastFootstepTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Uses Physics.CheckSphere to detect collision with ground layers.
    /// This is more efficient than continuous Raycasting for simple grounded checks.
    /// </summary>
    private void GroundedCheck()
    {
        // Define the center position of the grounding sphere check.
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);

        // Execute the physics check.
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
}