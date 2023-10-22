using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

//Create a base class that this and Colored Animator both derive from
public class PlayerAnimator : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float jumpAnimationLength;

    [Header("Idle")]
    [SerializeField] float maxIdleVelocity;

    Animator animator;

    bool isJumpPlaying;

    static readonly string runName = "Run_Player";
    static readonly string jumpName = "Jump_Player";
    static readonly string idleName = "Idle_Player";
    static readonly string joustName = "Joust_Player";

    static readonly int run = Animator.StringToHash(runName);
    static readonly int jump = Animator.StringToHash(jumpName);
    static readonly int idle = Animator.StringToHash(idleName);
    static readonly int joust = Animator.StringToHash(joustName);

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShouldPlayIdle(float playerMovement)
    {
        if (Mathf.Abs(playerMovement) > maxIdleVelocity)
            return;

        if (isJumpPlaying)
            return;

        PlayAnimation(idle);
    }

    public void ShouldPlayWalk(float playerMovement)
    {
        if (Mathf.Abs(playerMovement) < maxIdleVelocity)
            return;

        if (isJumpPlaying)
            return;

        PlayAnimation(run);
    }

    public void ShouldPlayJump(bool startedJump)
    {
        if (startedJump)
        {
            PlayAnimation(jump);

            StartCoroutine(StartJumpTimer());
        }
    }

    public void ShouldPlayJoust(bool startedJoust)
    {
        if (startedJoust)
        {
            PlayAnimation(joust);
        }
    }

    public bool IsAnimationPlaying(string animationName) => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);

    IEnumerator StartJumpTimer()
    {
        float timer = 0;

        while (true)
        {
            isJumpPlaying = true;
            timer += Time.deltaTime;

            if (timer >= jumpAnimationLength)
            {
                isJumpPlaying = false;
                yield break;
            }
            yield return null;
        }
    }

    protected void PlayAnimation(int animationToPlay)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.CrossFade(animationToPlay, 0, 0);
    }
}
