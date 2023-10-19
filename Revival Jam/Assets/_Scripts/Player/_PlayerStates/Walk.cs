using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "States/Player/Walk")]
public class Walk : State<CharacterController>
{
    [SerializeField] bool showDebug;

    [Header("Walk")]
    [SerializeField] float walkSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velocityPower;
    [SerializeField] float frictionAmount;

    [Header("Jump")]
    [SerializeField] float coyoteTimeLength;
    [SerializeField] float jumpBufferLength;

    [Header("Fall")]
    [SerializeField] float maxFallVelocity;

    [Header("Sound FX")]
    [SerializeField] float timeBetweenWalkSound;

    Transform transform;
    Rigidbody2D rigidbody;
    PlayerAnimator playerAnimator;
    Player player;


    float walkSoundTimer;
    float jumpTimer;
    float groundedTimer;

    float horizontalInput;

    float maxVerticalVelocity;

    bool walkOverride = false;

    public override void Enter(CharacterController parent)
    {
        base.Enter(parent);

        if (player == null)
            player = parent.GetComponent<Player>();
        if (rigidbody == null)
            rigidbody = parent.GetComponent<Rigidbody2D>();
        if (playerAnimator == null)
            playerAnimator = parent.GetComponent<PlayerAnimator>();
        if (transform == null)
            transform = parent.GetComponent<Transform>();

        maxVerticalVelocity = rigidbody.velocity.y;
    }

    public override void CaptureInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpTimer = jumpBufferLength;

    #if DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            walkOverride = !walkOverride;
        }
    #endif
    }

    public override void Update()
    {
        walkSoundTimer -= Time.deltaTime;
        groundedTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        if (player.IsGrounded)
            groundedTimer = coyoteTimeLength;

        KeepConstantVelocity();

        CheckAnimations();

        FlipPlayer();

        CheckWalkSound();
    }

    public override void FixedUpdate()
    {
        MovePlayer();
        ApplyFriction();
    }

    void CheckAnimations()
    {
        if (playerAnimator == null)
            return;

        playerAnimator.ShouldPlayWalk(rigidbody.velocity.x);
        playerAnimator.ShouldPlayIdle(rigidbody.velocity.x);
    }


    //Sets the scale of the player negative or positve, when they move left/right
    void FlipPlayer()
    {
        if (horizontalInput == 0 && rigidbody.velocity.x == 0)
            return;

        int multipler;

        //This is already in the MovePlayer function, so it might be more performant to put the checks there
        if (horizontalInput < 0 && rigidbody.velocity.x < 0)
            multipler = -1;
        else if (horizontalInput > 0 && rigidbody.velocity.x > 0)
            multipler = 1;
        else
            return;

        runner.transform.localScale = new Vector2(Mathf.Abs(runner.transform.localScale.x) * multipler, Mathf.Abs(runner.transform.localScale.y));
    }

    void CheckWalkSound()
    {
        if (!player.IsGrounded)
            return;

        if (Mathf.Abs(rigidbody.velocity.x) < 0.1f)
            return;

        if (Mathf.Abs(horizontalInput) < .1f)
            return;

        if (walkSoundTimer > 0)
            return;

        //CheckDebug(Mathf.Abs(rigidbody.velocity.x));

        WalkSound();
    }

    void WalkSound()
    {

    }

    public override void ChangeState()
    {
        bool isPlayer3dNull = FPSInput.Instance == null;
        bool shouldWalk = isPlayer3dNull || !FPSInput.Instance.ShouldWalk;

        bool isControlsManagerNull = ControlsManager.Instance == null;
        bool isControlConnected = isControlsManagerNull || ControlsManager.Instance.ConnectedControls.Contains(ControlsManager.Controls.Jump);

        if (jumpTimer > 0 && groundedTimer > 0 && (isControlConnected) && (shouldWalk))
        {
            jumpTimer = 0;
            groundedTimer = 0;

            runner.SetState(typeof(Jump));
        }

        if (Input.GetButtonDown("Attack") && player.IsGrounded)
        {
            runner.SetState(typeof(SimpleAttack));
        }
    }

    public override void Exit()
    {

    }

    //Makes sure the player can't gain more vertical velocity than they already have.
    //Prevents bouncing.
    //Note: Performance is poor, optimize using Clamp
    void KeepConstantVelocity()
    {
        if (rigidbody.velocity.y > maxVerticalVelocity)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, maxVerticalVelocity);
        else
            maxVerticalVelocity = rigidbody.velocity.y;

        if (maxVerticalVelocity < 0)
            maxVerticalVelocity = 0;
    }

    //Uses forces and math to move the player
    void MovePlayer()
    {
        float canWalkMod = 0;
        
        if (FPSInput.Instance != null)
        {
            canWalkMod = !FPSInput.Instance.ShouldWalk ? 1 : 0;
            CheckDebug($"Can walk if connected {canWalkMod}");
        }

        if (ControlsManager.Instance == null)
        {
            CheckDebug("Controls Manager not present : Assuming controls override");
            walkOverride = true;
        }
        else if (!ControlsManager.Instance.ConnectedControls.Contains(ControlsManager.Controls.Walk))
        {
            CheckDebug("Walk not connected");
            canWalkMod = 0;
        }

#if DEBUG
        if (walkOverride)
        {
            CheckDebug("Walk modifier active");
            canWalkMod = 1;
        }
#endif

        //Calculates the direction we wish to move in; this is our desired velocity
        float targetSpeed = horizontalInput * walkSpeed * canWalkMod;

        //Difference between the current and desired velocity
        float speedDifference = targetSpeed - rigidbody.velocity.x;

        //Changes our acceleration rate to suit the situation
        float acceleartionRate = (Mathf.Abs(targetSpeed > .01f ? acceleration : deceleration));

        //Applies acceleration to the speed difference, then raises it to a power, meaning acceleration increases with higher speeds
        //Multiplies it to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * acceleartionRate, velocityPower) * Mathf.Sign(speedDifference);

        rigidbody.AddForce(movement * Vector2.right);
    }

    //Applies force opposite to the player
    void ApplyFriction()
    {
        float amount = Mathf.Min(Mathf.Abs(rigidbody.velocity.x), Mathf.Abs(frictionAmount));

        amount *= Mathf.Sign(rigidbody.velocity.x);

        //Applies force against the player's movement direction
        rigidbody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    }

    void CheckDebug(string text)
    {
        if (showDebug)
            Debug.Log(text);
    }

    
}
