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
    [SerializeField] private float startFallHeight = 0f;


    public override void Activate(Player_Def player)
    {
        player.abilityDuration = abilityTimer;
        shouldBounce = true;

        // Record the height at which the player starts falling
        startFallHeight = player.transform.position.y;

        player.playerControls.Gameplay.Interact.performed += ctx =>
        {
            if (!player.isGrounded && player.currentlyActiveAnimal == this)
            {
                shouldBounce = true;
            }
        };
    }


    public override void UpdateAbilityState(Player_Def player)
    {
        if (shouldBounce && player.justLanded)
        {
            float fallDistance = startFallHeight - player.transform.position.y;
            float dynamicBounceForce = CalculateBounceForce(fallDistance);

            player.rb.velocity = new Vector3(player.rb.velocity.x, dynamicBounceForce, player.rb.velocity.z);
            shouldBounce = false;
            player.isAbilityActive = false;
            player.currentAbilityTimer = 0;
        }
        else if (player.isGrounded && player.isAbilityActive)
        {
            ResetAbility(player);
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
        shouldBounce = false;
    }

    public override bool AllowNormalJump()
    {
        return !shouldBounce; // Allow normal jump only if shouldBounce is false
    }
}



