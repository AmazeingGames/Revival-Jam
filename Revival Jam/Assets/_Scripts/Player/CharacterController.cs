using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ControlsManager;
using static PlayerFocus;

public class CharacterController : StateRunner<CharacterController>
{
    public static void CutJumpHeight(Rigidbody2D rigidbody, float jumpCutAmount)
    {
        if (rigidbody.velocity.y > 0)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * jumpCutAmount);
    }


    //Make thes functions extension methods
    public static void MovePlayer(Rigidbody2D rigidBody, float moveDirection, float speed, float accelerationAmount, float decelerationAmount, float velocityPower)
    {
        //Calculates the direction we wish to move in; this is our desired velocity
        float targetSpeed = moveDirection * speed;

        //Difference between the current and desired velocity
        float speedDifference = targetSpeed - rigidBody.velocity.x;

        //Changes our acceleration rate to suit the situation
        float acceleartionRate = (Mathf.Abs(targetSpeed > .01f ? accelerationAmount : decelerationAmount));

        //Applies acceleration to the speed difference, then raises it to a power, meaning acceleration increases with higher speeds
        //Multiplies it to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * acceleartionRate, velocityPower) * Mathf.Sign(speedDifference);

        rigidBody.AddForce(movement * Vector2.right);
    }

    //Applies force opposite to the player
    public static void ApplyFriction(Rigidbody2D rigidbody, float frictionAmount)
    {
        float amount = Mathf.Min(Mathf.Abs(rigidbody.velocity.x), Mathf.Abs(frictionAmount));

        amount *= Mathf.Sign(rigidbody.velocity.x);

        //Applies force against the player's movement direction
        rigidbody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    }

    //Makes sure the player can't gain more vertical velocity than they already have.
    //Prevents bouncing.
    //Note: Performance is likely poor, optimize using Clamp
    public static void KeepConstantVelocity(Rigidbody2D rigidbody, ref float verticalVelocityCeiling)
    {
        if (rigidbody.velocity.y > verticalVelocityCeiling)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, verticalVelocityCeiling);
        else
            verticalVelocityCeiling = rigidbody.velocity.y;

        if (verticalVelocityCeiling < 0)
            verticalVelocityCeiling = 0;
    }

    public static bool CanJump()
    {
        bool isFocusedOnArcade = IsFocusedOn(FocusedOn.Arcade);
        bool isJumpConnected = IsControlConnected(Controls.Jump);

        bool canJump = (isFocusedOnArcade && isJumpConnected);

        //Debug.Log($"CanJump : {canJump}");

        return canJump;
    }
}
