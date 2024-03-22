using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public Spider spider;
    [SerializeField] private Material normalMat;
    [SerializeField] private Material inRangeMat;
    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position,transform.position);
        
        if(distance < spider.grapplingRange)
        {
            GetComponent<MeshRenderer>().material = inRangeMat;
        }else GetComponent<MeshRenderer>().material = normalMat;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,spider.grapplingRange);
    }
}
