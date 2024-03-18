using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevels : MonoBehaviour
{
    private GameManager gM;
    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
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
            }else StartCoroutine(RestartLevel());
        }

        else if (other.CompareTag("Catch"))
        { //restart the level
            StartCoroutine(RestartLevel());
        }
    }


    IEnumerator RestartLevel(){
        yield return new WaitForSeconds(.2f);
        gM.ResetLevel();
    }
}
