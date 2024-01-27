using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Spider", menuName = "Animals/Spider")]
public class Spider : Animal
{
    public float grapplingRange = 10f;
    [SerializeField] private float grapplingSpeed = 15f;
    [SerializeField] private float grapplingCooldown = 2f; // Cooldown time in seconds
    private Vector3 grapplePoint;
    public bool isGrappling = false;
    public LayerMask whatIsGrappable;
    private float lastGrapplingTime = -Mathf.Infinity; // Initialize with a negative value
    private Player_Def playerRef; // Reference to the player

    public override void Activate(Player_Def player)
    {
        // The grappling logic will be handled in Player_Def
        // No need to subscribe to the Jump event here since it's handled in SubscribeToInputs
        playerRef = player;
        Debug.Log("Spider activated");
    }

    public override void ResetAbility(Player_Def player)
    {
        isGrappling = false;
        playerRef = null;
    }

    public void HandleJump(Player_Def player)
    {
        if (!player.isGrounded && !isGrappling && Time.time - lastGrapplingTime >= grapplingCooldown) 
        {
            if (FindClosestGrapplePoint(player.transform.position, out grapplePoint))
            {
                isGrappling = true;
                lastGrapplingTime = Time.time; // Update last grappling time
                player.StartCoroutine(GrapplingMovement(player));
            }
        }
    }

    public void HandleJumpRelease()
    {
        isGrappling = false;
    }


    private IEnumerator GrapplingMovement(Player_Def player)
    {
        Debug.Log("Grappling called");
        while (isGrappling)
        {
            Vector3 direction = (grapplePoint - player.transform.position).normalized;
            player.rb.velocity = direction * grapplingSpeed;
            if (Vector3.Distance(player.transform.position, grapplePoint) < 1f)
            {
                isGrappling = false;
                Debug.Log("Stopped grappling");
            }
            yield return null;
        }
    }

    private bool FindClosestGrapplePoint(Vector3 origin, out Vector3 closestPoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, grapplingRange, whatIsGrappable);
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = origin;
        closestPoint = Vector3.zero;
        bool foundPoint = false;

        foreach (Collider potentialTarget in hitColliders)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestPoint = potentialTarget.transform.position;
                foundPoint = true;
            }
        }

        return foundPoint;
    }
}

