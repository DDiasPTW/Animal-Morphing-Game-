using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;

public class ActivateCatchable : MonoBehaviour
{
    public GameObject catchable;

    void Awake()
    {
        catchable = GameObject.FindGameObjectWithTag("Catch");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            catchable.GetComponent<PathFollower>().canMove = true;
        }
    }
}
