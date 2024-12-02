using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeowSound : MonoBehaviour
{
    public AudioClip meow; 
    private AudioSource audioSource; 
    private float timer; 

    [Range(0f, 1f)] // Slider in the Inspector for volume control
    public float volume = 0.5f; // Default volume set to 50%

    // Start is called before the first frame update
    void Start()
    {
        // Add an AudioSource component to the GameObject if not already present
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the "meow" sound to the AudioSource
        audioSource.clip = meow;

        // Set the volume of the AudioSource
        audioSource.volume = volume;

        // Ensure the AudioSource doesn't play automatically
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the timer by the time passed since the last frame
        timer += Time.deltaTime;

        // Check if 10 seconds have passed
        if (timer >= 10f)
        {
            // Play the sound
            audioSource.Play();

            // Reset the timer
            timer = 0f;
        }
    }

    // Update the volume dynamically if changed in the Inspector
    private void OnValidate()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
