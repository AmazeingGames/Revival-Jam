using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : StateRunner<CharacterController>
{
    public static void CutJumpHeight(Rigidbody2D rigidbody, float jumpCutAmount)
    {
        if (rigidbody.velocity.y > 0)
            rigidbody.velocity *= jumpCutAmount;
    }
}
