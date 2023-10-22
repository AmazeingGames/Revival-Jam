using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Animations;
using UnityEngine;

//Create a base class that this and Colored Animator both derive from
public class PlayerAnimator : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float jumpAnimationLength;

    [Header("Idle")]
    [SerializeField] float maxIdleVelocity;

    [Header("Joust")]
    [SerializeField] float joustStartAnimationLength;
    [SerializeField] float joustFinishAnimationLength;

    Animator animator;

    bool isJumpPlaying;
    bool isJoustPlaying;

    static readonly string runName = "Walk_Player";
    static readonly string jumpName = "Jump_Player";
    static readonly string idleName = "Idle_Player";

    static readonly string joustStartName = "JoustStart_Player";
    static readonly string joustLoopName = "JoustLoop_Player";
    static readonly string joustFinishName = "JoustFinish_Player"; 

    static readonly int run = Animator.StringToHash(runName);
    static readonly int jump = Animator.StringToHash(jumpName);
    static readonly int idle = Animator.StringToHash(idleName);
    static readonly int joustStart = Animator.StringToHash(joustStartName);
    static readonly int joustLoop = Animator.StringToHash(joustLoopName);
    static readonly int joustFinish = Animator.StringToHash(joustFinishName);


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

        if (isJoustPlaying)
            return;

        PlayAnimation(idle);
    }

    public void ShouldPlayWalk(float playerMovement)
    {
        if (Mathf.Abs(playerMovement) < maxIdleVelocity)
            return;

        if (isJumpPlaying)
            return;

        if (isJoustPlaying)
            return;

        PlayAnimation(run);
    }

    public void ShouldPlayJump(bool playJump)
    {
        if (isJoustPlaying)
            return;

        if (playJump)
        {
            PlayAnimation(jump);

            StartCoroutine(StartJumpTimer());
        }
    }

    public void ShouldPlayJoust(bool playJoust)
    {
        if (playJoust)
        {
            if (isJoustPlaying)
                return;

            PlayAnimation(joustStart);

            StartCoroutine(StartJoust());
        }
        else
            StartCoroutine(StopJoust());
    }

    IEnumerator StopJoust()
    {

        StopCoroutine(StartJoust());

        PlayAnimation(joustFinish);

        yield return new WaitForSeconds(joustFinishAnimationLength);

        isJoustPlaying = false;
    }

    IEnumerator StartJoust()
    {
        isJoustPlaying = true;

        yield return new WaitForSeconds(joustStartAnimationLength);

        PlayAnimation(joustLoop);
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
