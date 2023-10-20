using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    [SerializeField] float invulnerabilityLength = .1f;
    float invulnerabilityTimer;

    private void Update()
    {
        if (MachineAbilities.IsMachineOn)
            invulnerabilityTimer -= Time.deltaTime;
        else
            invulnerabilityTimer = invulnerabilityLength;
    }

    protected override void Die()
    {
        ArcadeGameManager.Instance.UpdateArcadeState(ArcadeGameManager.ArcadeState.Lose);
    }

    public override void TakeDamage(int damageAmount)
    {
        if (invulnerabilityTimer >= 0)
            return;

        base.TakeDamage(damageAmount);
    }
}
