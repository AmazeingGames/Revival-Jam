using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] string targetTag;

    [SerializeField] bool shouldDebug;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDebug("Collison Entered");

        if (!CanDamage(collision))
            return;

        CheckDebug("Object is Damageable : Initiating Damage");

        InitiateDamage(collision, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            CheckDebug("Collison Exit");

            if (!CanDamage(collision))
                return;

            CheckDebug("Object is Damageable : Stopping Damage");

            InitiateDamage(collision, false);
        }
    }

    void InitiateDamage(Collision2D collision, bool startDamage)
    {
        Damageable damageable = collision.gameObject.GetComponent<Damageable>();

        if (startDamage)
            damageable.StartDamage(gameObject, damageAmount, timeBetweenDamage);
        else
            damageable.StopDamage(gameObject);
    }

    bool CanDamage(Collision2D collision)
    {
        if (collision.gameObject == null)
            return false;


        if (!collision.gameObject.CompareTag(targetTag))
        {
            //CheckDebug($"Does not have _{targetTag}_ tag | Instead has _{gameObject.tag}_ tag");
            return false;
        }

        //CheckDebug("Has Damagable script?");

        if (!collision.gameObject.TryGetComponent<Damageable>(out var damageable))
            return false;

        return damageable;
    }

    void CheckDebug(string text)
    {
        if (shouldDebug)
            Debug.Log(text);
    }
}
