using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxEdgeCollider : MonoBehaviour
{
    private Transform player; // Reference to the player object
    public float bufferDistance = 0.1f; // Buffer distance to prevent the player from sticking to the edges
    public float pushForce = 10f;
    private BoxCollider cubeCollider;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        cubeCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Vector3 closestPoint = cubeCollider.ClosestPoint(player.position);

        // Check if player is outside the collider
        if (!cubeCollider.bounds.Contains(player.position))
        {
            // Calculate direction to move player back inside
            Vector3 direction = player.position - closestPoint;
            direction.y = 0f; // Ignore vertical direction

            // Move player back inside with buffer distance
            player.position = closestPoint + direction.normalized * bufferDistance;
            Vector3 finalForce = direction.normalized * pushForce;
            player.GetComponent<Rigidbody>().AddForce(finalForce,ForceMode.Impulse);
        }
    }

}
