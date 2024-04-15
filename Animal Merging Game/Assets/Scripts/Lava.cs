using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private GameManager gM;
    private AudioSource aS;
    [SerializeField] private AudioClip hitSound;

    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        aS = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gM.levelFinished)
        {
            Narrator.Instance.TriggerLavaLines();

            aS.PlayOneShot(hitSound);

            StartCoroutine(RestartLevel(.25f));
        }
    }

    //Killable
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !gM.levelFinished)
        {
            Narrator.Instance.TriggerDyingLines();

            aS.PlayOneShot(hitSound);

            StartCoroutine(RestartLevel(.1f));
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
