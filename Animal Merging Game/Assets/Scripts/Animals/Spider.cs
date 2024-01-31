using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//TO DO:
//WHEN SWAPPING OUT OF SPIDER KEEP MOMENTUM OF THE SWING, FEELS VERY JANKY RIGHT NOW
//WHEN RELEASING THE SWING KEEP THE MOMENTUM, RIGHT NOW IT GETS FULLY KILLED BECAUSE OF THE WAY THE PLAYER SCRIPT HANDLES THE VELOCITY


[CreateAssetMenu(fileName = "Spider", menuName = "Animals/Spider")]
public class Spider : Animal
{
    public float grapplingRange = 10f;
    [SerializeField] private float grapplingCooldown = 2f; // Cooldown time in seconds
    private Vector3 grapplePoint;
    public bool isGrappling = false;
    public LayerMask whatIsGrappable;
    private float lastGrapplingTime = -Mathf.Infinity; // Initialize with a negative value
    private Player_Def playerRef; // Reference to the player
    [Header("SpringJoint values")]
    private SpringJoint springJoint;
    [SerializeField] private float maxDistance = 0.8f;
    [SerializeField] private float minDistance = 0.2f;

    [Tooltip("This parameter determines the stiffness of the spring"),
    SerializeField] private float spring = 4.5f; 

    [Tooltip("The damper reduces the spring's oscillation (bouncing back and forth)"),
     SerializeField] private float damper = 7f;

    [Tooltip("This multiplies the mass of the object for the spring calculation, affecting how the spring's force acts on the player"),
    SerializeField] private float massScale = 4.5f;
    private float swingStartHeight;

    public override void Activate(Player_Def player)
    {
        // The grappling logic will be handled in Player_Def
        // No need to subscribe to the Jump event here since it's handled in SubscribeToInputs
        playerRef = player;
        lastGrapplingTime = -Mathf.Infinity; //reset this value so the player can immediately swing
        if(player.gameObject.GetComponent<SpringJoint>() != null)
        {
            Destroy(player.gameObject.GetComponent<SpringJoint>());
        }
    }

    public override void ResetAbility(Player_Def player)
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
        }

        isGrappling = false;
        playerRef = null;

        lastGrapplingTime = -Mathf.Infinity;
    }

    public void HandleJump(Player_Def player)
    {
        if (!player.isGrounded && !isGrappling && Time.time - lastGrapplingTime >= grapplingCooldown)
        {
            if (FindClosestGrapplePoint(player.transform.position, out grapplePoint))
            {
                StartSwing(player);
            }
        }
    }

    public void HandleJumpRelease()
    {
        if (isGrappling)
        {
            StopSwing();
        }
    }

    private void StartSwing(Player_Def player)
    {
        springJoint = player.gameObject.AddComponent<SpringJoint>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);
        
        springJoint.maxDistance = distanceFromPoint * maxDistance;
        springJoint.minDistance = distanceFromPoint * minDistance;

        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.massScale = massScale;

        swingStartHeight = player.transform.position.y;
        isGrappling = true;
        lastGrapplingTime = Time.time;
    }

    private void StopSwing()
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
        }
        isGrappling = false;
    }

    private bool FindClosestGrapplePoint(Vector3 origin, out Vector3 closestPoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, grapplingRange, whatIsGrappable);
        closestPoint = Vector3.zero;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider potentialTarget in hitColliders)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - origin;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestPoint = potentialTarget.transform.position;
            }
        }

        return closestPoint != Vector3.zero;
    }
}

