using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using static ControlsManager;
using static PlayerFocus;

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
    [SerializeField] float jumpCutAmount;

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
            playerAnimator = parent.GetComponentInChildren<PlayerAnimator>();
        if (transform == null)
            transform = parent.GetComponent<Transform>();

        maxVerticalVelocity = rigidbody.velocity.y;
    }

    public override void CaptureInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpTimer = jumpBufferLength;

        if (Input.GetButtonUp("Jump"))
            CharacterController.CutJumpHeight(rigidbody, jumpCutAmount);

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

        jumpTimer -= Time.deltaTime;

        CharacterController.KeepConstantVelocity(rigidbody, ref maxVerticalVelocity);

        CheckAnimations();

        FlipPlayer();

        CheckWalkSound();
    }


    bool CanWalkLeft() => CanWalkDirection(isRight: false);

    bool CanWalkRight() => CanWalkDirection(isRight: true);

    bool CanWalkDirection(bool isRight)
    {
        Controls walkDirection = isRight ? Controls.WalkRight : Controls.WalkLeft;
        string directionText = $"{walkDirection}"[4..];

        bool isFocusedOnArcade = IsFocusedOn(FocusedOn.Arcade);

        bool isWalkDirectionConnected = IsControlConnected(walkDirection);

        bool canWalkDirection = (isFocusedOnArcade && isWalkDirectionConnected);

        Debug.Log($"canWalk{directionText} : {canWalkDirection}");

        return canWalkDirection;
    }

    

    public override void FixedUpdate()
    {
        MovePlayer();
        CharacterController.ApplyFriction(rigidbody, frictionAmount);
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
        if (jumpTimer > 0 && Player.Instance.LastGroundedTime <= coyoteTimeLength && CharacterController.CanJump())
        {
            jumpTimer = 0;

            runner.SetState(typeof(Jump));
        }

        if (Input.GetButtonDown("Attack") && player.IsGrounded && CharacterController.CanJoust())
        {
            runner.SetState(typeof(Joust));
        }
    }

    public override void Exit()
    {

    }

    void MovePlayer()
    {
        float canWalkRightMod = 0;
        float canWalkLeftMod = 0;

        if (PlayerFocus.Instance != null)
        {
            canWalkRightMod = CanWalkRight() ? 1 : 0;
            canWalkLeftMod = CanWalkLeft() ? 1 : 0;

            CheckDebug($"If connected - canWalkRight {canWalkRightMod} | canWalkLeft {canWalkLeftMod}");
        }

        if (ControlsManager.Instance == null)
        {
            CheckDebug("Controls Manager not present : Assuming controls override");
            walkOverride = true;
        }
        

#if DEBUG
        if (walkOverride)
        {
            CheckDebug("Walk modifier active");
            canWalkLeftMod = 1;
            canWalkRightMod = 1;
        }
#endif

        float walkModToUse = horizontalInput < 0 ? canWalkLeftMod : canWalkRightMod;

        CharacterController.MovePlayer(rigidbody, horizontalInput * walkModToUse, walkSpeed, acceleration, deceleration, velocityPower);
    }

    void CheckDebug(string text)
    {
        if (showDebug)
            Debug.Log(text);
    }
}
