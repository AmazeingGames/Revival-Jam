using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    [Header("Health")]
    [SerializeField] float invulnerabilityLength = .1f;
    float invulnerabilityTimer = -1;

    private void Update()
    {
        return;

        if (MachineAbilities.Instance.IsMachineOn)
            invulnerabilityTimer -= Time.deltaTime;
        else
            invulnerabilityTimer = invulnerabilityLength;
    }

    protected override void Die()
    {
        if (ArcadeGameManager.Instance == null)
            return;
        ArcadeGameManager.Instance.UpdateArcadeState(ArcadeGameManager.ArcadeState.Lose);
    }

    public override void TakeDamage(int damageAmount)
    {
        if (invulnerabilityTimer >= 0)
            return;

        base.TakeDamage(damageAmount);
    }
}
