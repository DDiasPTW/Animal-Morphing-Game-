using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;

public class ActivateCatchable : MonoBehaviour
{
    public GameObject catchable;
    private GameManager gM;
    private AudioSource aS;
    [SerializeField] private AudioClip startSound;
    private bool soundPlayed = false;
    

    void Awake()
    {
        catchable = GameObject.FindGameObjectWithTag("Catch");
        
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        gM.canStartTimer = false; Debug.Log(gM.canStartTimer);

        aS = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            if(catchable != null)
            {
                catchable.GetComponent<PathFollower>().canMove = true;
                
            }
            gM.canStartTimer = true;
            
            if(!soundPlayed){
                aS.PlayOneShot(startSound);
                soundPlayed = true;
            }
            
        }
    }
}
