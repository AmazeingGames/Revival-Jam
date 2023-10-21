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
}
