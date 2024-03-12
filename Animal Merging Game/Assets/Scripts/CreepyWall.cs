using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepyWall : MonoBehaviour
{
    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.LookAt(player.transform);
    }
}
