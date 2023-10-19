using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : DealDamage
{
    private void OnEnable()
    {
        AttackHitbox.Hit += HandleHit;
    }

    private void OnDisable()
    {
        AttackHitbox.Hit -= HandleHit;
    }

    void HandleHit(GameObject gameObject, bool isStartingDamage)
    {
        Debug.Log($"Handle hit : {gameObject.name} | start damage {isStartingDamage}");
        ShouldInitiateDamage(gameObject, isStartingDamage);
    } 
}
