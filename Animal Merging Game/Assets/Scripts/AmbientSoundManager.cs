using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager instance; // Singleton instanc


    public AudioSource ambientSound;
    public AudioSource birdSound;
    [Header("Ambient sound")]
    [SerializeField] private List<AudioClip> ambientSounds = new List<AudioClip>();
    public float minVolume = 0.2f;
    public float maxVolume = 0.8f;
    public float changeInterval = 5f; // Time interval between volume changes
    public float changeDuration = 2f; // Duration of the volume change
    private AudioClip currentAmbientSound; // Track the current ambient sound
    [Header("Bird sound")]
    [SerializeField] private List<AudioClip> birdSounds = new List<AudioClip>();
    public float minBirdVolume = 0.2f;
    public float maxBirdVolume = 0.8f;
    public float changeBirdInterval = 5f; // Time interval between volume changes
    public float changeBirdDuration = 2f; // Duration of the volume change

    private AudioClip currentBirdSound; // Track the current bird sound

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



    private void Start()
    {
        // Choose a random ambient sound at the beginning
        currentAmbientSound = ambientSounds[Random.Range(0, ambientSounds.Count)];
        // Choose a random bird sound at the beginning
        currentBirdSound = birdSounds[Random.Range(0, birdSounds.Count)];

        // Start coroutines for both sounds
        StartCoroutine(RandomizeVolume(ambientSound, currentAmbientSound, minVolume, maxVolume, changeInterval, changeDuration));
        StartCoroutine(RandomizeBirdVolume(birdSound, currentBirdSound, minBirdVolume, maxBirdVolume, changeBirdInterval, changeBirdDuration));

    }

    private IEnumerator RandomizeVolume(AudioSource aS, AudioClip clip, float minVol, float maxVol, float interval, float duration)
    {
        aS.clip = clip;
        aS.time = Random.Range(0f, clip.length); // Start playing at a random time within the clip
        aS.Play();

        while (true)
        {
            float currentVolume = Random.Range(minVol, maxVol);
            float targetVolume = Random.Range(minVol, maxVol);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                aS.volume = Mathf.Lerp(currentVolume, targetVolume, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            aS.volume = targetVolume; // Ensure the volume is set to the exact target volume
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator RandomizeBirdVolume(AudioSource aS, AudioClip clip, float minVol, float maxVol, float interval, float duration)
    {
        aS.clip = clip;
        aS.time = Random.Range(0f, clip.length); // Start playing at a random time within the clip
        aS.Play();

        while (true)
        {
            float currentVolume = Random.Range(minVol, maxVol);
            float targetVolume = Random.Range(minVol, maxVol);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                aS.volume = Mathf.Lerp(currentVolume, targetVolume, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            aS.volume = targetVolume; // Ensure the volume is set to the exact target volume
            yield return new WaitForSeconds(interval);
        }
    }

}
