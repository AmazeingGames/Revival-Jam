using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Damageable
{
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
