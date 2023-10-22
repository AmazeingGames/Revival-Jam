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

    [Header("Animation")]
    [SerializeField] Animator animator;

    void Start()
    {
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
        animator.CrossFade(animationToPlay, 0, 0);
    }
}
