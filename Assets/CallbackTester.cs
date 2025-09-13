using UnityEngine;

/// <summary>
/// Handles callback events from Wwise.
/// </summary>
public class CallbackTester : MonoBehaviour
{
    /// <summary>
    /// Called when the Wwise callback is triggered.
    /// </summary>
    public void TestCallback()
    {
        Debug.Log("¡El callback ha sido activado  por la gran puta LOCOOOOOOOOOOO!");
    }
}