using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class AnimationsHandler : MonoBehaviour
{
    private Animator anim;
    [HideInInspector] public Player_Def player;
    private bool justSwapped = false;


    private int idleAnim;
    private int moveAnim;
    private int jumpAnim;
    private int landAnim;
    private int fallAnim;

    [SerializeField] private string idleAnimName;
    [SerializeField] private string moveAnimName;
    [SerializeField] private string jumpAnimName;
    [SerializeField] private string landAnimName;
    [SerializeField] private string fallAnimName;



    [Header("Audio")]
    private AudioSource aS;
    [SerializeField] private AudioClip footStep;


    void Awake()
    {
        anim = GetComponent<Animator>();
        aS = GetComponent<AudioSource>();
        CacheAnimationHashes();
    }

    public void NotifyFormSwapped()
    {
        justSwapped = true;
    }

    private void CacheAnimationHashes()
    {
        idleAnim = Animator.StringToHash(idleAnimName);
        moveAnim = Animator.StringToHash(moveAnimName);
        jumpAnim = Animator.StringToHash(jumpAnimName);
        landAnim = Animator.StringToHash(landAnimName);
        fallAnim = Animator.StringToHash(fallAnimName);
    }

    void Update()
    {
        // If we just swapped and the current state is one where we want to skip replay,
        // set a flag to indicate waiting for the next state. Otherwise, play the animation normally.
        if (justSwapped)
        {
            if (ShouldSkipImmediateReplay(player.CurrentState))
            {
                // Here, instead of resetting justSwapped immediately, wait for a new state to play the animation
            }
            else
            {
                justSwapped = false; // Reset the flag for states that don't need to skip replay
                PlayAnimationBasedOnState(player.CurrentState); // Play animation for states that can immediately update
            }
        }
        else
        {
            PlayAnimationBasedOnState(player.CurrentState);
        }
    }

    void PlayAnimationBasedOnState(Player_Def.PlayerState currentState)
    {
        // Reset all parameters to false to ensure only one animation state is active at a time
        //ResetAnimationParameters();

        switch (player.CurrentState)
        {
            case Player_Def.PlayerState.Idle:
                anim.Play(idleAnim);
                //anim.SetBool("isIdle",true);
                break;
            case Player_Def.PlayerState.Moving:
                anim.Play(moveAnim);
                //anim.SetBool("isMoving",true);
                break;
            case Player_Def.PlayerState.Jumping:
                anim.Play(jumpAnim);
                //anim.SetBool("isJumping",true);
                break;
            case Player_Def.PlayerState.Landing:
                anim.Play(landAnim);
                //anim.SetBool("isLanding",true);
                break;
            case Player_Def.PlayerState.Falling:
                anim.Play(fallAnim);
                //anim.SetBool("isFalling",true);
                break;
            default:
                anim.Play(idleAnim);
                break;
        }
    }

    bool ShouldSkipImmediateReplay(Player_Def.PlayerState currentState)
    {
        //States I wish to skip replaying immediately after a swap
        return currentState == Player_Def.PlayerState.Jumping;
    }

    public void PlayStepSound()
    {
        aS.PlayOneShot(footStep);
    }
}
