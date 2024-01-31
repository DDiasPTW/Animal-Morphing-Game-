using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlyingSquirrel", menuName = "Animals/FlyingSquirrel")]
public class FlyingSquirrel : Animal
{
    [SerializeField] private float glideGravityScale = 0.5f; // Reduced gravity while gliding
    [SerializeField] private float airControl = 10f;
    [SerializeField] private float glideForwardSpeed = 10f;
    [SerializeField] private float onTransitionDuration = 0.5f; // Duration of speed change when activating the animal
    private float originalAirControl;
    private float originalGravityScale;
    private float originalForwardSpeed;
    private bool isGliding = false;
    private Coroutine speedChangeCoroutine;

    public override void Activate(Player_Def player)
    {
        originalForwardSpeed = player.moveSpeed;
        originalGravityScale = player.normalGravityScale; // Store the original gravity scale
        originalAirControl = player.airControlFactor;
    }

    public override void ResetAbility(Player_Def player)
    {
        StopGlide(player); // Ensure gliding is stopped when ability is reset
    }

    public void HandleJump(Player_Def player)
    {
        // Only allow gliding when in mid-air and not already gliding
        if (!player.isGrounded && !isGliding)
        {
            if (speedChangeCoroutine != null)
            {
                player.StopCoroutine(speedChangeCoroutine);
                speedChangeCoroutine = null;
            }
            StartGlide(player);
        }
    }

    public void HandleJumpRelease(Player_Def player)
    {
        if (isGliding)
        {
            StopGlide(player);
        }
    }

    private void StartGlide(Player_Def player)
    {
        player.normalGravityScale = glideGravityScale; // Reduce gravity to glide
        player.airControlFactor = airControl;
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
        }
        speedChangeCoroutine = player.StartCoroutine(ChangeSpeed(player, glideForwardSpeed, onTransitionDuration));
        isGliding = true;
    }

    private void StopGlide(Player_Def player)
    {
        player.normalGravityScale = originalGravityScale; // Reset gravity to normal
        player.airControlFactor = originalAirControl;
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
            speedChangeCoroutine = null;
        }
        player.moveSpeed = originalForwardSpeed;
        isGliding = false;
    }

    //Gradually change the player speed instead of instantly
    private IEnumerator ChangeSpeed(Player_Def player, float targetSpeed, float timer)
    {
        float startSpeed = player.moveSpeed;
        float elapsedTime = 0;

        while (elapsedTime < timer)
        {
            player.moveSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / timer);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.moveSpeed = targetSpeed; // Ensure exact target speed is set at the end
    }
}

