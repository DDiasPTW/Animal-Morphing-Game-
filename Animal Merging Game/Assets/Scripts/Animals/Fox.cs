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
        player.jumpForce = jumpForce; // Immediately change jump force
        // No need to set isAbilityActive or currentAbilityTimer
    }

    public override void ResetAbility(Player_Def player)
    {
        player.jumpForce = playerJumpForce; // Reset the jump force
    }
}


