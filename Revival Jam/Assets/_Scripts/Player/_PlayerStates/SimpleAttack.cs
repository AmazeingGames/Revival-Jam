using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/SimpleAttack")]
public class SimpleAttack : State<CharacterController>
{
    [SerializeField] float attackLength;
    [SerializeField] float jumpBufferLength;

    [Header("Debug")]
    [SerializeField] bool showDebug;

    GameObject attackHitbox;

    float jumpBufferTimer;
    float attackTimer;

    Rigidbody2D rigidbody2D;

    public override void Enter(CharacterController parent)
    {
        base.Enter(parent);

        CheckDebug("attack!");

        if (rigidbody2D == null)
            rigidbody2D = parent.GetComponent<Rigidbody2D>();
        if (attackHitbox == null)
            attackHitbox = parent.transform.Find("AttackHitbox").gameObject;

        attackHitbox.SetActive(true);
        attackTimer = attackLength;
        jumpBufferTimer = 0;
        rigidbody2D.velocity = Vector3.zero;
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
    }

    public override void FixedUpdate()
    {
        
    }

    public override void ChangeState()
    {
        if (attackTimer > 0)
            return;
            
        if (jumpBufferTimer > 0)
            runner.SetState(typeof(Jump));
        else
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
