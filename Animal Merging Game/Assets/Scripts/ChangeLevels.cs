using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevels : MonoBehaviour
{
    private GameManager gM;
    [SerializeField] private AudioClip enterSound;
    private AudioSource aS;
    private bool triggered = false;
    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        aS = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if(gM != null) return;
        if(gM == null){
            gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gM.canEndLevel)
            {
                gM.levelFinished = true;
                other.GetComponent<Player_Def>().currentState = Player_Def.PlayerState.Idle;
                aS.PlayOneShot(enterSound);
            }else StartCoroutine(RestartLevel());
        }

        else if (other.CompareTag("Catch"))
        { //restart the level
            if(!triggered){
                Narrator.Instance.TriggerEndCatchable();
                triggered = true;
            }
            
            StartCoroutine(RestartLevel());
        }
    }


    IEnumerator RestartLevel(){
        yield return new WaitForSeconds(.2f);
        gM.ResetLevel();
    }
}
