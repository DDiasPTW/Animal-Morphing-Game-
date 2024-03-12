using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEditor;

public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager instance; // Singleton instance
    public AudioSource ambientSound;
    public AudioSource birdSound;
    [Header("Ambient sound")]
    [SerializeField] private List<AudioClip> ambientSounds = new List<AudioClip>();
    private int ambientSoundPlayCount;
    private AudioClip currentAmbientSound; // Track the current ambient sound
    [Header("Bird sound")]
    [SerializeField] private List<AudioClip> birdSounds = new List<AudioClip>();
    private int birdSoundPlayCount;
    private AudioClip currentBirdSound; // Track the current bird sound
    [Header("Other")]
    [SerializeField] private string MainMenuScene;
    private bool soundStarted = false;

    private void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scene loads
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }
    }



    private void Update()
    {
        if (SceneManager.GetActiveScene().name == MainMenuScene)
        {
            return;
        }
        else
        {
            if (!soundStarted)
            {
                soundStarted = true;
                // Choose a random ambient sound at the beginning
                currentAmbientSound = ambientSounds[Random.Range(0, ambientSounds.Count)];
                // Choose a random bird sound at the beginning
                currentBirdSound = birdSounds[Random.Range(0, birdSounds.Count)];

                // Start coroutines for both sounds
                StartCoroutine(RandomizeVolume(ambientSound, currentAmbientSound));
                StartCoroutine(RandomizeBirdVolume(birdSound, currentBirdSound));
            }
        }
    }

    private IEnumerator RandomizeVolume(AudioSource aS, AudioClip clip)
    {
        aS.clip = clip;
        aS.time = Random.Range(0f, clip.length); // Start playing at a random time within the clip
        aS.Play();
        yield return null;

        // Increment the play count and check if it's time to switch to a new clip
        ambientSoundPlayCount++;
        if (ambientSoundPlayCount >= 5) // Change the threshold as needed
        {
            currentAmbientSound = GetRandomClip(ambientSounds);
            aS.clip = currentAmbientSound;
            ambientSoundPlayCount = 0;
        }
    }

    private IEnumerator RandomizeBirdVolume(AudioSource aS, AudioClip clip)
    {
        aS.clip = clip;
        aS.time = Random.Range(0f, clip.length); // Start playing at a random time within the clip
        aS.Play();
        yield return null;

        // Increment the play count and check if it's time to switch to a new clip
        birdSoundPlayCount++;
        if (birdSoundPlayCount >= 5) // Change the threshold as needed
        {
            currentBirdSound = GetRandomClip(birdSounds);
            aS.clip = currentBirdSound;
            birdSoundPlayCount = 0;
        }
    }

    private AudioClip GetRandomClip(List<AudioClip> clips)
    {
        // If the list is empty or null, return null
        if (clips == null || clips.Count == 0)
            return null;

        // Choose a random clip
        return clips[Random.Range(0, clips.Count)];
    }

}
