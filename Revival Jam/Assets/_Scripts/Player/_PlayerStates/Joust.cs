using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Joust")]
public class Joust : State<CharacterController>
{
    [Header("MoveArrow Player")]
    [SerializeField] float dashSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velocityPower;
    [SerializeField] float frictionAmount;
    [SerializeField] bool lockVerticalVelocity = true;

    [Header("Joust")]
    [SerializeField] float joustLength;
    [SerializeField] float jumpBufferLength;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float coyoteTimeLength;
    [SerializeField] float jumpCutAmount;

    [Header("Sound FX")]
    [SerializeField] float timeBetweenJoustSFX;
    [SerializeField] float timeBetweenWalkSFX;

    [Header("Debug")]
    [SerializeField] bool showDebug;

    float walkSFXTimer;

    float jumpBufferTimer;
    float facingDirection;
    float verticalVelocityCeiling;

    Rigidbody2D rigidbody;
    Transform transform;
    GameObject attackHitbox;
    PlayerAnimator playerAnimator;
    PlayerSFX playerSFX;
    Player player;

    bool first = true;

    public override void Enter(CharacterController parent)
    {
        base.Enter(parent);

        Debug.Log("Joust!");

        if (rigidbody == null)
            rigidbody = parent.GetComponent<Rigidbody2D>();
        if (attackHitbox == null)
            attackHitbox = parent.transform.Find("AttackHitbox").gameObject;
        if (transform == null)
            transform = parent.transform;
        if (playerAnimator == null)
            playerAnimator = parent.GetComponentInChildren<PlayerAnimator>();
        if (playerSFX == null)
            playerSFX = parent.GetComponent<PlayerSFX>();
        if (player == null)
            player = parent.GetComponent<Player>();

        if (first)
        {
            WalkCleanup();
            first = false;
        }

        playerSFX.StartJoustSound(timeBetweenJoustSFX);
        attackHitbox.SetActive(true);
        jumpBufferTimer = 0;
        Player.Instance.ShouldIncrementJoustTimer(true);
        playerAnimator.ShouldPlayJoust(true);
        //rigidbody.velocity = Vector3.zero;

        verticalVelocityCeiling = rigidbody.velocity.y;
    }

    public override void CaptureInput()
    {
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferLength;
    }

    public override void Update()
    {
        jumpBufferTimer -= Time.deltaTime;

        facingDirection = transform.localScale.x < 0 ? -1 : 1;

        CharacterController.KeepConstantVelocity(rigidbody, ref verticalVelocityCeiling);

        walkSFXTimer -= Time.deltaTime;

        CheckWalkSound();
    }

    void CheckWalkSound()
    {
        if (!player.IsGrounded)
            return;

        if (walkSFXTimer > 0)
            return;

        PlayWalkSFX();
    }

    void PlayWalkSFX()
    {
        walkSFXTimer = timeBetweenWalkSFX;
        AudioManager.TriggerAudioClip(Player.Instance.GetWalkSound(), transform);
    }


    public override void FixedUpdate()
    {
        CharacterController.MovePlayer(rigidbody, facingDirection, dashSpeed, acceleration, deceleration, velocityPower);
        CharacterController.ApplyFriction(rigidbody, frictionAmount);
    }

    public override void ChangeState()
    {
        if (jumpBufferTimer > 0 && Player.Instance.LastGroundedTime <= coyoteTimeLength && CharacterController.CanJump())
            runner.SetState(typeof(JoustJump));

        if (Player.Instance.JoustTimer <= joustLength)
            return;

        WalkCleanup();
        runner.SetState(typeof(Walk));
    }

    void WalkCleanup()
    {
        attackHitbox.SetActive(false);

        playerAnimator.ShouldPlayJoust(false);
        playerSFX.StopJoustSound();
        walkSFXTimer = timeBetweenWalkSFX;

        Player.Instance.ResetJoustTimer();
        Player.Instance.ShouldIncrementJoustTimer(false);
    }

    public override void Exit()
    {
        
    }
}
