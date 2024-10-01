// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using UnityEngine;

/// <summary>
/// Handles the game end trigger by quitting the game when the player collides with the trigger collider.
/// </summary>
public class GameEndTrigger : MonoBehaviour
{
    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// </summary>
    /// <param name="otherCollider">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        HandleGameEnd(); // Handle the end of the game when the trigger is activated.
    }

    /// <summary>
    /// Handles the game end by quitting the game or stopping play mode in the Unity Editor.
    /// </summary>
    private static void HandleGameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing the game in the Unity Editor
#else
        Application.Quit(); // Quit the application if not in the Unity Editor
#endif
    }
}
