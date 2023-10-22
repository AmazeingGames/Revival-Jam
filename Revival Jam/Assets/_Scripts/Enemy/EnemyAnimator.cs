using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;
public class EnemyAnimator : MonoBehaviour
{
    static readonly string walkName = "Walk_Skeleton";
    static readonly string dieName = "Death_Skeleton";
    static readonly string idleName = "Idle_Skeleton";

    static readonly int walk = Animator.StringToHash(walkName);
    static readonly int die = Animator.StringToHash(dieName);
    static readonly int idle = Animator.StringToHash(idleName);

    public enum EnemyAnimation { Walk, Die, Idle }
    public Dictionary<EnemyAnimation, int> AnimationTypeToHash;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        AnimationTypeToHash = new()
        {
            { EnemyAnimation.Walk,  walk },
            { EnemyAnimation.Die,   die},
            { EnemyAnimation.Idle,  idle },
        };
    }

    public void PlayAnimation()
    {

    }

    public void PlayAnimation(EnemyAnimation animation) => PlayAnimation(AnimationTypeToHash[animation]);

    protected void PlayAnimation(int animationToPlay)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.CrossFade(animationToPlay, 0, 0);
    }
}
