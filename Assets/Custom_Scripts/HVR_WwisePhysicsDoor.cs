using UnityEngine;
using AK.Wwise;
using HurricaneVR.Framework.Components;

public class HVR_WwisePhysicsDoor : HVRPhysicsDoor
{
    [Header("Wwise Events")]
    public AK.Wwise.Event doorOpenedWwiseEvent;
    public AK.Wwise.Event doorClosedWwiseEvent;
    public AK.Wwise.Event doorLockedWwiseEvent;
    public AK.Wwise.Event doorUnlockedWwiseEvent;

    // Override the OnDoorOpened method to add Wwise functionality
    protected override void OnDoorOpened()
    {
        base.OnDoorOpened(); // This calls the original HVR logic first.

        if (doorOpenedWwiseEvent != null)
        {
            doorOpenedWwiseEvent.Post(gameObject);
        }
    }

    // Override the OnDoorClosed method
    protected override void OnDoorClosed()
    {
        base.OnDoorClosed();

        if (doorClosedWwiseEvent != null)
        {
            doorClosedWwiseEvent.Post(gameObject);
        }
    }

    // Override the public Lock method to add a Wwise event
    public override void Lock()
    {
        base.Lock(); // Call the original HVR lock logic.

        if (doorLockedWwiseEvent != null)
        {
            doorLockedWwiseEvent.Post(gameObject);
        }
    }

    // Override the public Unlock method to add a Wwise event
    public override void Unlock()
    {
        base.Unlock(); // Call the original HVR unlock logic.

        if (doorUnlockedWwiseEvent != null)
        {
            doorUnlockedWwiseEvent.Post(gameObject);
        }
    }
}