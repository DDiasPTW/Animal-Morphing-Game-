using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public Spider spider;

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,spider.grapplingRange);
    }
}
