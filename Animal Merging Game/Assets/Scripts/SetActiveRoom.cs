using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveRoom : MonoBehaviour
{
    [SerializeField] private bool hasPlayer = false;


    void Update()
    {
        if(hasPlayer){ gameObject.transform.GetChild(0).gameObject.SetActive(true);} else gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            hasPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")){
            hasPlayer = false;
        }
    }
}
