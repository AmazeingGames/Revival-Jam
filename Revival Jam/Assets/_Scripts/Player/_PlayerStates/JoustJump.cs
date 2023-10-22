using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/JoustJump")]
public class JoustJump : Jump
{
    public override void Enter(CharacterController parent)
    {
        base.Enter(parent);

        Debug.Log("Joust jump!");
    }

    public override void ChangeState()
    {
        if (switchTimer <= 0)
            runner.SetState(typeof(Joust));
    }
}
