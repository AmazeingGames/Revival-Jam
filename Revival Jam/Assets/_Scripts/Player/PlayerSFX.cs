using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;
using static AudioManager.EventSounds;

public class PlayerSFX : MonoBehaviour
{
    bool isJoustPlaying = false;

    public void StartJoustSound(float timeBetweenSounds)
    {
        if (isJoustPlaying)
        {
            Debug.Log("Joust already playing");
            return;
        }

        Debug.Log("Start Joust sound");
        StartCoroutine(PlayJoustSounds(timeBetweenSounds));
    }

    public void StopJoustSound()
    {
        Debug.Log("Stop joust");
        StopAllCoroutines();
        isJoustPlaying = false;
    }

    IEnumerator PlayJoustSounds(float timeBetweenSounds)
    {
        while (true)
        {
            Debug.Log("joust running");
            isJoustPlaying = true;
            AudioManager.Instance.TriggerAudioClip(PlayerJoust, transform);
            yield return new WaitForSeconds(timeBetweenSounds);
        }  
    }
}
