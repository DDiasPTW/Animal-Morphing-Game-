using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHandler : MonoBehaviour
{
    private Animator anim;
    [HideInInspector] public Player_Def player;
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
    void Awake()
    {
        anim = GetComponent<Animator>();
        CacheAnimationHashes();
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
            default:
                anim.Play(idleAnim);
                break;
        }
    }
}
