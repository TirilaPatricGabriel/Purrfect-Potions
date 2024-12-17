using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeowSound : MonoBehaviour
{
    public AudioClip meow; 
    private AudioSource audioSource; 
    private float timer; 

    [Range(0f, 1f)] 
    public float volume = 0.5f; 

    void Start()
    {
        // audio source with 'meow'
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = meow;

        audioSource.volume = volume;

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // every 10 seconds play sound
        if (timer >= 10f)
        {
            audioSource.Play();

            timer = 0f;
        }
    }

    private void OnValidate()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
