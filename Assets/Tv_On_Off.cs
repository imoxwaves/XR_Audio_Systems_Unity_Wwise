using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Handles callback events from Wwise.
/// </summary>
public class Tv_On_Off : MonoBehaviour
{
    // A reference to the Video Player component on the same GameObject.
    private VideoPlayer videoPlayer;

    /// <summary>
    /// Gets a reference to the Video Player component when the script starts.
    /// </summary>
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    /// <summary>
    /// Called when the Wwise callback is triggered.
    /// </summary>
    public void Video_Activado()
    {
        // Check if the Video Player component exists.
        if (videoPlayer != null)
        {
            // Play the video.
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("Error: VideoPlayer component not found on this GameObject.");
        }
    }

    /// <summary>
    /// Called when the Wwise callback is triggered to stop the video.
    /// </summary>
    public void StopVideoCallback()
    {
        // Check if the Video Player component exists.
        if (videoPlayer != null)
        {
            // Stop the video.
            videoPlayer.Stop();
        }
        else
        {
            Debug.LogError("Error: VideoPlayer component not found on this GameObject.");
        }
    }

}