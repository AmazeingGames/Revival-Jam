using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "States/Player/Dash")]
public class Dash : State<CharacterController>
{
    //[SerializeField] TrailRenderer trailRenderer;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashLength;
    [SerializeField] float maxVelocityYCut = 3;
    [SerializeField] float timeToLerp = .5f;

    Vector2 dashingDirection;
    Rigidbody2D rigidbody;
    float dashTimer;

    float regularGravity;

    float current;
    float target;
    float time;

    public override void Enter(CharacterController parent)
    {
        base.Enter(parent);

        if (rigidbody == null)
            rigidbody = parent.GetComponent<Rigidbody2D>();

        //trailRenderer.emitting = true;

        Debug.Log("DASH!");

        dashTimer = dashLength;

        regularGravity = rigidbody.gravityScale;
        rigidbody.gravityScale = 0;

        dashingDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (dashingDirection == Vector2.zero)
            dashingDirection = new Vector2(parent.transform.localScale.x, 0).normalized;
    }

    public override void CaptureInput()
    {
        
    }

    public override void Update()
    {
        dashTimer -= Time.deltaTime;

        rigidbody.velocity = dashingDirection * dashSpeed;
    }

    public override void FixedUpdate()
    {

    }

    public override void ChangeState()
    {
        if (dashTimer <= 0)
            runner.SetState(typeof(Walk));
    }

    public override void Exit()
    {
        //trailRenderer.emitting = false;

        rigidbody.gravityScale = regularGravity;

        rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y > maxVelocityYCut ? maxVelocityYCut : rigidbody.velocity.y);
    }
}
