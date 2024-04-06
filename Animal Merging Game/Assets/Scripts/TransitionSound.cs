using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSound : MonoBehaviour
{
    private AudioSource aS;
    private GameManager gM;
    [SerializeField] private AudioClip appearAudio;
    [SerializeField] private AudioClip appearSecondAudio;
    [SerializeField] private GameObject nextLevelButtonEnd;
    [SerializeField] private GameObject mainMenuButtonEnd;

    void Awake()
    {
        aS = GetComponent<AudioSource>();
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        nextLevelButtonEnd.SetActive(false);
        mainMenuButtonEnd.SetActive(false);
    }

    public void PlayOne()
    {
        if(gM.starsToActivate >= 1){
            aS.volume = .1f;
            aS.PlayOneShot(appearAudio);

            if(gM.starsToActivate == 1){
                Narrator.Instance.TriggerEndLevelLines(1);
            }
        }
        
    }
    public void PlayTwo()
    {
        if(gM.starsToActivate >= 2){
            aS.PlayOneShot(appearAudio);
           

            if(gM.starsToActivate == 2){
                Narrator.Instance.TriggerEndLevelLines(2);
            }
        }
    }
    public void PlayThree()
    {
        if(gM.starsToActivate >= 3){
            aS.PlayOneShot(appearAudio);
            

            if(gM.starsToActivate == 3){
                Narrator.Instance.TriggerEndLevelLines(3);
            }
        }
    }
    public void PlayFour()
    {
        if(gM.starsToActivate >= 4){
            aS.PlayOneShot(appearAudio);
            

            if(gM.starsToActivate == 4){
                Narrator.Instance.TriggerEndLevelLines(4);
            }
        }
    }
    public void PlayFive()
    {
        if(gM.starsToActivate >= 5){
            aS.PlayOneShot(appearAudio);


            if(gM.starsToActivate == 5){
                Narrator.Instance.TriggerEndLevelLines(5);
            }
             
        }
    }
    public void PlaySix()
    {
        if(gM.starsToActivate >= 6){
            aS.pitch = .5f;
            aS.PlayOneShot(appearAudio);


            if(gM.starsToActivate == 6){
                Narrator.Instance.TriggerEndLevelLines(6);
            }
        }
    }

    public void PlayAppear(){
        aS.volume = .1f;
        aS.pitch = 1f;
        aS.PlayOneShot(appearSecondAudio);
    }

    public void GoNext(){
        gM.LoadNextLevel();
    }


    public void CheckNextLevel()
    {
        // Check if there's no next scene
        if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            // There is no next scene
            nextLevelButtonEnd.SetActive(false);
            mainMenuButtonEnd.SetActive(true);
        }else
        {
            nextLevelButtonEnd.SetActive(true);
            mainMenuButtonEnd.SetActive(false);
        }
    }
}
