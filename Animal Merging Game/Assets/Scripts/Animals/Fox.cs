using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fox", menuName = "Animals/Fox")]
public class Fox : Animal
{
    [SerializeField] private float jumpForce = 15f;
    private float playerJumpForce;

    public override void Activate(Player_Def player)
    {
        playerJumpForce = player.jumpForce;
        player.abilityDuration = abilityTimer;  // Set the duration for the Fox ability
        player.jumpForce = jumpForce;          // Increase the jump force
        player.isAbilityActive = true;          // Mark the ability as active
        player.currentAbilityTimer = abilityTimer;
    }

    public override void ResetAbility(Player_Def player)
    {
        player.jumpForce = playerJumpForce;    // Reset the jump force
    }

    public override void UpdateAbilityState(Player_Def player)
    {
        // This method will be called every frame by Player_Def
        // If the ability duration has expired, reset the ability
        if (player.currentAbilityTimer <= 0)
        {
            ResetAbility(player);
            player.isAbilityActive = false;      // Mark the ability as inactive
        }
    }
}

