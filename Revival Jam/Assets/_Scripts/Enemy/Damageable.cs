using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;
using static AudioManager.OneShotSounds;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] bool shouldDebug;

    [Header("Sound FX")]
    [SerializeField] OneShotSounds takeDamageSound;

    readonly Dictionary<GameObject, Coroutine> DamageCoroutine = new();

    public virtual void StartDamage(GameObject gameObject, int damageAmount, float timeBetweenDamage)
    {
        CheckDebug("    Damage Coroutine Started");

        Coroutine coroutine = StartCoroutine(Damage(damageAmount, timeBetweenDamage));

        DamageCoroutine.Add(gameObject, coroutine);
    }

    public virtual void StopDamage(GameObject gameObject)
    {
        if (DamageCoroutine.TryGetValue(gameObject, out var coroutine) && coroutine != null)
        {
            CheckDebug("Damage Coroutine Stopped");
            StopCoroutine(coroutine);
            DamageCoroutine.Remove(gameObject);
        }
        else
            CheckDebug("Could not stop coroutine");
    }

    public virtual void TakeDamage(int damageAmount)
    {
        CheckDebug($"Took {damageAmount} damage");

        health -= damageAmount;

        TriggerAudioClip(takeDamageSound, transform);

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator Damage(int damageAmount, float timeBetweenDamage)
    {
        while (true)
        {
            TakeDamage(damageAmount);

            yield return new WaitForSeconds(timeBetweenDamage);
        }
    }

    protected abstract void Die();

    void CheckDebug(string text)
    {
        if (shouldDebug)
            Debug.Log(text);
    }
}
