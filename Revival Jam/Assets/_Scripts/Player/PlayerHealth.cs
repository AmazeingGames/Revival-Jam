using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    [SerializeField] float invulnerabilityLength = .1f;
    float invulnerabilityTimer;

    private void Update()
    {
        if (ArcadeGameManager.IsMachineOn)
            invulnerabilityTimer -= Time.deltaTime;
        else
            invulnerabilityTimer = invulnerabilityLength;
    }

    protected override void Die()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.Lose);
    }

    public override void TakeDamage(int damageAmount)
    {
        if (invulnerabilityTimer >= 0)
            return;

        base.TakeDamage(damageAmount);
    }
}
