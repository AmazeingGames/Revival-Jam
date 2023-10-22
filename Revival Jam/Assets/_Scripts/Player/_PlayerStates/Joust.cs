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

    [Header("Debug")]
    [SerializeField] bool showDebug;


    float jumpBufferTimer;
    float attackTimer;
    float facingDirection;
    float verticalVelocityCeiling;

    Rigidbody2D rigidbody2D;
    Transform transform;
    GameObject attackHitbox;

    public override void Enter(CharacterController parent)
    {
        base.Enter(parent);

        CheckDebug("Joust!");

        if (rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if (attackHitbox == null)
            attackHitbox = parent.transform.Find("AttackHitbox").gameObject;
        if (transform == null)
            transform = parent.transform;

        attackHitbox.SetActive(true);
        attackTimer = joustLength;
        jumpBufferTimer = 0;
        rigidbody2D.velocity = Vector3.zero;

        verticalVelocityCeiling = rigidbody2D.velocity.y;
    }

    public override void CaptureInput()
    {
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferLength;
    }

    public override void Update()
    {
        jumpBufferTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        facingDirection = transform.localScale.x < 0 ? -1 : 1;

        CharacterController.KeepConstantVelocity(rigidbody2D, ref verticalVelocityCeiling);
    }

    public override void FixedUpdate()
    {
        CharacterController.MovePlayer(rigidbody2D, facingDirection, dashSpeed, acceleration, deceleration, velocityPower);
        CharacterController.ApplyFriction(rigidbody2D, frictionAmount);
    }

    public override void ChangeState()
    {
        if (attackTimer > 0)
            return;
             
        runner.SetState(typeof(Walk));
    }

    public override void Exit()
    {
        attackHitbox.SetActive(false);
    }

    void CheckDebug(string text)
    {
        if (showDebug)
            Debug.Log(text);
    }
}
