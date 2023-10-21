using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : StateRunner<CharacterController>
{
    public static void CutJumpHeight(Rigidbody2D rigidbody, float jumpCutAmount)
    {
        if (rigidbody.velocity.y > 0)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * jumpCutAmount);
    }

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
}
