using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionSound : MonoBehaviour
{
    private AudioSource aS;
    private GameManager gM;
    [SerializeField] private List<AudioClip> starSounds = new List<AudioClip>();

    void Awake()
    {
        aS = GetComponent<AudioSource>();
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    public void PlayOne()
    {
        if(gM.starsToActivate >= 1){
            aS.PlayOneShot(starSounds[0]);
        }
        
    }
    public void PlayTwo()
    {
        if(gM.starsToActivate >= 2){
            aS.PlayOneShot(starSounds[1]);
        }
    }
    public void PlayThree()
    {
        if(gM.starsToActivate >= 3){
            aS.PlayOneShot(starSounds[2]);
        }
    }
    public void PlayFour()
    {
        if(gM.starsToActivate >= 4){
            aS.PlayOneShot(starSounds[3]);
        }
    }
    public void PlayFive()
    {
        if(gM.starsToActivate >= 5){
            aS.PlayOneShot(starSounds[4]);
        }
    }
    public void PlaySix()
    {
        if(gM.starsToActivate >= 6){
            aS.PlayOneShot(starSounds[5]);
        }
    }
}
