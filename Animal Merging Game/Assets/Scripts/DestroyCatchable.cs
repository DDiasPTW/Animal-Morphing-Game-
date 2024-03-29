using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCatchable : MonoBehaviour
{
    [SerializeField] private GameObject catchable;

    void Awake()
    {
        catchable = GameObject.FindGameObjectWithTag("Catch");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            if(catchable == null) return;
            if(other.transform.position.z > catchable.gameObject.transform.position.z){
                catchable.GetComponent<Catch>().Caught();
            }
            else return;   
        }
    }
}
