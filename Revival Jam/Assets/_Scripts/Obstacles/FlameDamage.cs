using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDamage : DealDamage
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckDebug("Collison Entered");

        if (!ShouldInitiateDamage(collision.gameObject, true))
            return;

        CheckDebug("Object is Damageable : Initiating Damage");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckDebug("Collison Exit");

        if (!ShouldInitiateDamage(collision.gameObject, false))
            return;

        CheckDebug("Object is Damageable : Stopping Damage");
    }
}
