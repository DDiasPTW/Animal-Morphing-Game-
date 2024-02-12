using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "Cheetah", menuName = "Animals/Cheetah")]
public class Cheetah : Animal
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float onTransitionDuration = 0.5f; // Duration of speed change when activating the animal
    [SerializeField] private float offTransitionDuration = 0.5f; // Duration of speed change when de activating the animal
    private float originalPlayerMoveSpeed;
    private float originalPlayerAccelerationTime;
    private Coroutine speedChangeCoroutine;


    public override void Activate(Player_Def player)
    {
        originalPlayerMoveSpeed = player.startMoveSpeed;
        originalPlayerAccelerationTime = player.accelerationTime;
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
        }
        player.accelerationTime = accelerationTime;
        speedChangeCoroutine = player.StartCoroutine(ChangeSpeed(player, moveSpeed, onTransitionDuration));
    }

    public override void ResetAbility(Player_Def player)
    {
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
        }

        speedChangeCoroutine = player.StartCoroutine(ChangeSpeed(player, originalPlayerMoveSpeed, offTransitionDuration));
        player.accelerationTime = originalPlayerAccelerationTime;
    }

    //Gradually change the player speed instead of instantly, allowing for more combo possibilities and less frustration
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


