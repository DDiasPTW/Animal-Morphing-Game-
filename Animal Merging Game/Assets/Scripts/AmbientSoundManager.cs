using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager instance; // Singleton instance
    [SerializeField] private AudioSource aS;
    //[SerializeField] private List<AudioClip> mainMenuMusic = new List<AudioClip>();
    [SerializeField] private List<AudioClip> gameplayAmbience = new List<AudioClip>();

    private bool isFadingOut = false;
    private float fadeOutDuration = 1f; // Duration of fade out in seconds


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
        PlayRandomClip();
    }

    private void Update()
    {
        if (!aS.isPlaying && !isFadingOut)
        {
            StartCoroutine(FadeOutAndPlayNext());
        }
    }

    private IEnumerator FadeOutAndPlayNext()
    {
        isFadingOut = true;
        float startVolume = aS.volume;

        while (aS.volume > 0)
        {
            aS.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        PlayRandomClip();
        isFadingOut = false;
        aS.volume = startVolume;
    }

    private void PlayRandomClip()
    {
        if(!aS.isPlaying)
        {
            AudioClip randomClip = gameplayAmbience[Random.Range(0, gameplayAmbience.Count)];
            aS.clip = randomClip;
            aS.Play();
        }
    }

}
