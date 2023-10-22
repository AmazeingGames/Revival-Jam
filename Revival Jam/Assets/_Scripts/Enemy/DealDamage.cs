using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public abstract class DealDamage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float timeBetweenDamage;
    [SerializeField] string targetTag;

    [SerializeField] bool shouldDebug;

    [Header("Sound FX")]
    [SerializeField] EventSounds dealDamageSound;

    protected virtual void InitiateDamage(GameObject _gameObject, bool startDamage)
    {
        Damageable damageable = _gameObject.GetComponent<Damageable>();

        if (startDamage)
            damageable.StartDamage(gameObject, damageAmount, timeBetweenDamage);
        else
            damageable.StopDamage(gameObject);
    }

    protected virtual void InitiateDamage(Collision2D collision, bool startDamage)
    {
        InitiateDamage(collision.gameObject, startDamage);
    }

    protected virtual bool ShouldInitiateDamage(GameObject _gameObject, bool isStartingDamage)
    {
        if (_gameObject == null)
            return false;

        if (!_gameObject.CompareTag(targetTag))
            return false;

        if (!_gameObject.TryGetComponent<Damageable>(out var damageable))
            return false;

        InitiateDamage(_gameObject, isStartingDamage);

        return damageable;
    }

    protected bool ShouldInitiateDamage(Collision2D collision, bool startDamage)
    {
        return ShouldInitiateDamage(collision.gameObject, startDamage);
    }

    protected void CheckDebug(string text)
    {
        if (shouldDebug)
            Debug.Log(text);
    }
}
