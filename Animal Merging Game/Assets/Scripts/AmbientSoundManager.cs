using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager instance; // Singleton instance
    [SerializeField] private AudioSource aS;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private List<AudioClip> gameplayMusic = new List<AudioClip>();

    private Coroutine fadeCoroutine; // Coroutine for fading
    private bool isFading = false; // Flag to track if fading coroutine is running
    private int lastPlayedIndex = -1; // Index of the last played gameplay music

    private bool _playMusic = true;


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

        aS = GetComponent<AudioSource>();
    }


    void Start()
    {
        PlayMainMenuMusic();
    }

    public void PlayMainMenuMusic()
    {
        StopAndFade(mainMenuMusic);
    }

    public void PlayRandomGameplayMusic()
    {
        StopAndFade(GetRandomGameplayMusic());
    }

    
    private AudioClip GetRandomGameplayMusic()
    {
        // Ensure the next random track is not the same as the last one played
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, gameplayMusic.Count);
        }
        while (randomIndex == lastPlayedIndex);

        lastPlayedIndex = randomIndex; // Update the index of the last played track
        return gameplayMusic[randomIndex];
    }

    private void StopAndFade(AudioClip nextClip)
    {
        if (!isFading) // Check if fading coroutine is not already running
        {
            isFading = true; // Set flag to indicate that fading coroutine is running

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOutAndIn(nextClip));
        }
    }

    private IEnumerator FadeOutAndIn(AudioClip nextClip)
    {
        //Debug.Log("Started");
        const float fadeDuration = 0.25f; // Adjust as needed
        float startVolume = aS.volume;

        // Fade out
        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            aS.volume = Mathf.Lerp(startVolume, 0.0f, t / fadeDuration);
            //Debug.Log("Fade out");
            yield return null;
        }

        aS.volume = 0.0f;
        aS.Stop();

        // Play next clip
        aS.clip = nextClip;
        aS.Play();
        //Debug.Log("Playing");

        // Fade in
        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            aS.volume = Mathf.Lerp(0.0f, startVolume, t / fadeDuration);
            //Debug.Log("Fade in");
            yield return null;
        }

        aS.volume = startVolume;

        isFading = false; // Reset the flag after finishing the fading coroutine
    }




    private void Update()
    {
        if (!aS.isPlaying && aS.clip != mainMenuMusic)
        {
            PlayRandomGameplayMusic();
        }
    }

}
