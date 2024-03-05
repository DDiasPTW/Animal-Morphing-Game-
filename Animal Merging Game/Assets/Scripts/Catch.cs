using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catch : MonoBehaviour
{
    private GameManager gM;
    private AudioSource aS;
    [SerializeField] private AudioClip catchClip;
    [SerializeField] private GameObject catchParticles;
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
        if(other.CompareTag("Player") && !gM.canEndLevel)
        {
            //allow the player to end the level
            gM.canEndLevel = true;
            
            //disable the model
            transform.GetChild(0).gameObject.SetActive(false);

            //spawn the particles
            GameObject pS = Instantiate(catchParticles,transform);
            StartCoroutine(DestroyParticles(pS));
            
            //play an audio queue for extra feedback
            aS.PlayOneShot(catchClip);

            //destroy the gameObject
            StartCoroutine(DestroySelf());
        }
    }

    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(1f);
        Destroy(particles);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(catchClip.length);
        Destroy(gameObject);
    }
}
