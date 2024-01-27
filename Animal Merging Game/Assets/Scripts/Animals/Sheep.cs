using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sheep", menuName = "Animals/Sheep")]
public class Sheep : Animal
{
    [SerializeField] private bool shouldBounce = false;
    [SerializeField] private float bounceForceMultiplier = 2f;
    [SerializeField] private float minBounceForce = 5f;
    [SerializeField] private float maxBounceForce = 20f;
    [SerializeField] private float minFallHeightForBounce = 3f;



    public override void Activate(Player_Def player)
    {
        // If the player is in the air, prepare for a potential bounce
        if (!player.isGrounded)
        {
            shouldBounce = true;
        }
        else
        {
            shouldBounce = false;
        }
    }

    public override void UpdateAbilityState(Player_Def player)
    {
        if (player.justLanded)
        {
            float fallDistance = player.peakJumpHeight - player.transform.position.y;
            if (fallDistance >= minFallHeightForBounce)
            {
                float dynamicBounceForce = CalculateBounceForce(fallDistance);
                player.isBouncing = true;
                player.rb.velocity = new Vector3(player.rb.velocity.x, dynamicBounceForce, player.rb.velocity.z);
            }
            else
            {
                // If the fall height is not enough for a bounce, reset the flags
                player.isBouncing = false; // Reset the bouncing flag if no bounce occurs
            }

            
            // Reset peak height and shouldBounce flag after landing
            player.peakJumpHeight = player.transform.position.y;
            shouldBounce = false;
        }
    }

    private float CalculateBounceForce(float fallDistance)
    {
        // Example calculation - customize this based on your game's physics
        float force = Mathf.Clamp(fallDistance * bounceForceMultiplier, minBounceForce, maxBounceForce);
        Debug.Log(force);
        return force;
    }

    public override void ResetAbility(Player_Def player)
    {
        // Reset peak height when resetting ability
        player.peakJumpHeight = player.transform.position.y;
    }


    public override bool AllowNormalJump()
    {
        return !shouldBounce; // Allow normal jump only if shouldBounce is false
    }
}



