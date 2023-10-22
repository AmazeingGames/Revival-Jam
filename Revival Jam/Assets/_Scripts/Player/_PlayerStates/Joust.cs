using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Joust")]
public class Joust : State<CharacterController>
{
    [Header("Move Player")]
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

    [Header("Debug")]
    [SerializeField] bool showDebug;

    float jumpBufferTimer;
    float facingDirection;
    float verticalVelocityCeiling;

    Rigidbody2D rigidbody;
    Transform transform;
    GameObject attackHitbox;

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

        if (first)
        {
            WalkCleanup();
            first = false;
        }

        attackHitbox.SetActive(true);
        jumpBufferTimer = 0;
        Player.Instance.ShouldIncrementJoustTimer(true);
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
             
        runner.SetState(typeof(Walk));
        WalkCleanup();
    }

    void WalkCleanup()
    {
        attackHitbox.SetActive(false);
        Player.Instance.ResetJoustTimer();
        Player.Instance.ShouldIncrementJoustTimer(false);
    }

    public override void Exit()
    {
        
    }
}
