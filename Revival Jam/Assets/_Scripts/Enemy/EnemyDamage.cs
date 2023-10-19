using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : DealDamage
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDebug("Collison Entered");

        if (!ShouldInitiateDamage(collision, true))
            return;

        CheckDebug("Object is Damageable : Initiating Damage");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CheckDebug("Collison Exit");

        if (!ShouldInitiateDamage(collision, false))
            return;

        CheckDebug("Object is Damageable : Stopping Damage");
    }
}
