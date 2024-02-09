using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDamage : DealDamage
{
    StudioEventEmitter emitter;

    private void Awake()
    {
        emitter = AudioManager.Instance.InitializeEventEmitter(AudioManager.LoopingAudio.FireWall, gameObject);
    }

    private void OnEnable()
    {
        Debug.Log("Played emitter");
        emitter.Play();
    }

    private void OnDisable()
    {
        Debug.Log("Stopped emitter");

        emitter.Stop();
    }

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
