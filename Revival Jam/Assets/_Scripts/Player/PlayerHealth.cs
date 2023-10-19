using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    protected override void Die()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.Lose);
    }
}
