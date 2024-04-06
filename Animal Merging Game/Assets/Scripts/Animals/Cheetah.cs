using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "Cheetah", menuName = "Animals/Cheetah")]
public class Cheetah : Animal
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float accelTime = .3f;
    [SerializeField] private float onTransitionDuration = 0.5f; // Duration of speed change when activating the animal
    private Coroutine speedChangeCoroutine;



    // Inside your Cheetah class
    public override void Activate(Player_Def player)
    {
        player.accelerationTime = accelTime;
        // Set the new speed and start the speed change coroutine
        speedChangeCoroutine = player.StartCoroutine(ChangeSpeed(player, moveSpeed, onTransitionDuration));
    }

    public override void ResetAbility(Player_Def player)
    {
        // Stop the ongoing speed change coroutine if any
        if (speedChangeCoroutine != null)
        {
            player.StopCoroutine(speedChangeCoroutine);
        }
    }
}


