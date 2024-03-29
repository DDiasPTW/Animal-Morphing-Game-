using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lava : MonoBehaviour
{

    private GameManager gM;
    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gM.levelFinished)
        {
            Narrator.Instance.TriggerLavaLines();
            StartCoroutine(RestartLevel(.2f));
        }
    }

    //Killable
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !gM.levelFinished)
        {
            Narrator.Instance.TriggerDyingLines();
            StartCoroutine(RestartLevel(0f));
        }
    }

    IEnumerator RestartLevel(float delay){
        yield return new WaitForSeconds(delay);
        if(!gM.levelFinished)
        {
            gM.ResetLevel();
        }
        
    }

}
