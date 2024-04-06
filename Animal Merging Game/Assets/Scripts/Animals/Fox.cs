using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Fox", menuName = "Animals/Fox")]
public class Fox : Animal
{
    [SerializeField] private float jumpForce = 15f;
    private float playerJumpForce;

    [Header("ChangeSpeed")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float accelTime = .2f;
    [SerializeField] private float onTransitionDuration = 0.5f; // Duration of speed change when activating the animal
    private Coroutine speedChangeCoroutine;

    // Inside your Fox class
    public override void Activate(Player_Def player)
    {
        //Change jump height
        playerJumpForce = player.jumpForce;
        player.jumpForce = jumpForce; // Immediately change jump force

        // Set the new speed and start the speed change coroutine
        player.accelerationTime = accelTime;
        speedChangeCoroutine = player.StartCoroutine(ChangeSpeed(player, moveSpeed, onTransitionDuration));
    }

    public override void ResetAbility(Player_Def player)
    {
        player.jumpForce = playerJumpForce; // Reset the jump force

        // Stop the ongoing speed change coroutine if any
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
        }

    }
}


