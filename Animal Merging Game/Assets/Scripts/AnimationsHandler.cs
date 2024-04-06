using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationsHandler : MonoBehaviour
{
    [Header("Animations")]
    private Animator anim;
    [HideInInspector] public Player_Def player;
    private bool justSwapped = false;

    private int idleAnim;
    private int moveAnim;
    private int jumpAnim;
    private int landAnim;
    private int fallAnim;
    private int swingAnim;

    [SerializeField] private string idleAnimName;
    [SerializeField] private string moveAnimName;
    [SerializeField] private string jumpAnimName;
    [SerializeField] private string landAnimName;
    [SerializeField] private string fallAnimName;
    [SerializeField] private string swingAnimName;


    [Header("Audio")]
    private AudioSource aS;
    [SerializeField] private AudioClip footStep;
    [SerializeField][Range(0, 1)] private float footStepVolume;
    [SerializeField] private AudioClip landing;
    [SerializeField][Range(0, 1)] private float landingVolume;
    [SerializeField] private AudioClip jumping;
    [SerializeField][Range(0, 1)] private float jumpVolume;
    [SerializeField] private AudioClip[] swinging;
    [SerializeField][Range(0, 1)] private float swingVolume;
    [Header("Particles")]
    public GameObject landingParticles;
    public GameObject jumpParticles;


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
        swingAnim = Animator.StringToHash(swingAnimName);
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
                break;
            case Player_Def.PlayerState.Moving:
                anim.Play(moveAnim);
                break;
            case Player_Def.PlayerState.Jumping:
                anim.Play(jumpAnim);
                break;
            case Player_Def.PlayerState.Landing:
                anim.Play(landAnim);
                break;
            case Player_Def.PlayerState.Falling:
                anim.Play(fallAnim);
                break;
            case Player_Def.PlayerState.Swinging:
                anim.Play(swingAnim);
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
        aS.volume = footStepVolume;
        aS.PlayOneShot(footStep);
    }
    public void PlayLandSound()
    {
        aS.volume = landingVolume;
        aS.PlayOneShot(landing);

        GameObject particle = Instantiate(landingParticles, player.groundCheckPosition);
        particle.transform.SetParent(null);
        StartCoroutine(DestroyParticles(particle));
    }
    public void PlayJumpSound()
    {
        aS.volume = jumpVolume;
        aS.PlayOneShot(jumping);

        GameObject particle = Instantiate(jumpParticles, player.groundCheckPosition);
        particle.transform.SetParent(null);
        StartCoroutine(DestroyParticles(particle));
    }

    public void PlaySwingSound()
    {
        aS.volume = swingVolume;
        int i = UnityEngine.Random.Range(0, swinging.Length);
        aS.PlayOneShot(swinging[i]);
    }

    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(.5f);
        Destroy(particles);
    }
}
