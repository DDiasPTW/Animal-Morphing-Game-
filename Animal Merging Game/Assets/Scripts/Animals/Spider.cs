using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Spider", menuName = "Animals/Spider")]
public class Spider : Animal
{
    #region GrappleValues
    public float grapplingRange = 10f;
    [SerializeField] private float grapplingCooldown = 2f; // Cooldown time in seconds
    [SerializeField] private float exitBoostMultiplier = 1.2f;
    [SerializeField] private float maxVelocity = 70f;
    [HideInInspector] public Vector3 grapplePoint;
    public bool isSwinging = false;
    public LayerMask whatIsGrappable;
    [SerializeField] private float lastGrapplingTime = -Mathf.Infinity; // Initialize with a negative value
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
    #endregion

    #region ChangeSpeed
    [Header("ChangeSpeed")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float accelTime = .2f;
    [SerializeField] private float onTransitionDuration = 0.5f; // Duration of speed change when activating the animal
    private Coroutine speedChangeCoroutine;
    #endregion

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

        // Set the new speed and start the speed change coroutine
        player.accelerationTime = accelTime;
        speedChangeCoroutine = player.StartCoroutine(ChangeSpeed(player, moveSpeed, onTransitionDuration));
    }

    public override void ResetAbility(Player_Def player)
    {
        if(isSwinging) StopSwing(player);
        ResetSwing(); 

         // Stop the ongoing speed change coroutine if any
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
        }
    }

    private void ResetSwing()
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
        }

        isSwinging = false;
        playerRef = null;

        lastGrapplingTime = -Mathf.Infinity;
        
    }

    public void HandleJump(Player_Def player)
    {
        if (!player.isGrounded && !isSwinging && Time.time - lastGrapplingTime >= grapplingCooldown)
        {
            if (FindClosestGrapplePoint(player.transform.position, out grapplePoint))
            {
                StartSwing(player);
            } else if(FindDistanceGrapplePointNarratorLine(player.transform.position, out grapplePoint))
            {
                Narrator.Instance.TriggerFailSwingLines();
            } 
        }
    }

    public void HandleJumpRelease(Player_Def player)
    {
        if (isSwinging)
        {
            StopSwing(player);
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

        isSwinging = true; 
    }

    private void StopSwing(Player_Def player)
    {
        if (springJoint != null)
        {
            // Calculate the swing's final velocity vector based on the direction of the swing and the current speed
            Vector3 swingVelocity = player.rb.velocity;

            // Add a boost to the swing's exit velocity to simulate a more dynamic release
            swingVelocity *= exitBoostMultiplier;
            swingVelocity = Vector3.ClampMagnitude(swingVelocity,maxVelocity);
            // Apply the calculated velocity to the player's Rigidbody to maintain momentum after the swing
            player.rb.velocity = swingVelocity;
            
            //Debug.Log(swingVelocity.magnitude);

            // Clean up the SpringJoint component
            Destroy(springJoint);
            springJoint = null;
        }
        lastGrapplingTime = Time.time;
        isSwinging = false;
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


    private bool FindDistanceGrapplePointNarratorLine(Vector3 origin, out Vector3 closestPoint) //Minimum distance from a grapple point for the narrator to comment
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, grapplingRange * 1.25f, whatIsGrappable);
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

