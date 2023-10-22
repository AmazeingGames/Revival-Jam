using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
 class AudioManager : Singleton<AudioManager>
{
    [field: Header("Arcade Game Ambience")]
    [SerializeField] EventReference castleAmbience;
    [SerializeField] EventReference castleGlitchAmbience;
    [SerializeField] EventReference forestAmbience;
    [SerializeField] EventReference forestGlitchAmbience;

    [field: Header("Arcade Game Player SFX")]
    [SerializeField] EventReference playerJoust;
    [SerializeField] EventReference playerDamage;
    [SerializeField] EventReference playerGrassFootsteps;
    [SerializeField] EventReference playerStoneFootsteps;

    [field: Header("Arcade Cabinet SFX")]
    [SerializeField] EventReference circuitCablePlug;
    [SerializeField] EventReference circuitCableUnplug;
    [SerializeField] EventReference arcadeOn;
    [SerializeField] EventReference arcadeOff;
    [SerializeField] EventReference circuitPanelOpen;

    [field: Header("3D Game")]
    [SerializeField] EventReference player3DFootsteps;

    public enum EventSounds { Null, CastleAmbience, CastleGlitchAmbience, ForestAmbience, ForestGlitchAmbience, PlayerJoust, PlayerDamage, PlayerGrassFootsteps, PlayerStoneFootsteps, CircuitCablePlug, CircuitCableUnplug, ArcadeOn, ArcadeOff, CircuitPanelOpen, Player3DFootsteps }

    Dictionary<EventSounds, EventReference> SoundTypeToReference;

    readonly List<EventInstance> EventInstances = new();

    void Start()
    {
        SoundTypeToReference = new()
        {
            { EventSounds.CastleAmbience, castleAmbience },
            { EventSounds.CastleGlitchAmbience, castleGlitchAmbience},
            { EventSounds.ForestAmbience, forestAmbience},
            { EventSounds.ForestGlitchAmbience, forestGlitchAmbience },
            { EventSounds.PlayerJoust, playerJoust},
            { EventSounds.PlayerDamage, playerDamage},
            { EventSounds.PlayerGrassFootsteps, playerGrassFootsteps},
            { EventSounds.PlayerStoneFootsteps, playerStoneFootsteps},
            { EventSounds.CircuitCablePlug, circuitCablePlug},
            { EventSounds.CircuitCableUnplug, circuitCableUnplug},
            { EventSounds.ArcadeOn, arcadeOn },
            { EventSounds.ArcadeOff, arcadeOff},
            { EventSounds.CircuitPanelOpen, circuitPanelOpen },
            { EventSounds.Player3DFootsteps, player3DFootsteps },
        };
    }

    void OnDestroy()
    {
        CleanUp();
    }

    public void TriggerAudioClip(EventSounds sound, GameObject origin) => TriggerAudioClip(sound, origin.transform.position);

    public void TriggerAudioClip(EventSounds sound, Transform origin) => TriggerAudioClip(sound, origin.position);

    public void TriggerAudioClip(EventSounds sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");

        if (sound == EventSounds.Null)
            return;

        TriggerAudioClip(SoundTypeToReference[sound], origin);
    }

    void TriggerAudioClip(EventReference sound, Vector3 origin)
    {
        RuntimeManager.PlayOneShot(sound, origin);
    }

    void CleanUp()
    {
        foreach (var eventInstance in EventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    /*
    //Creates the instance
    EventInstance CreateEventInstance(EventSounds eventSound) => CreateEventInstance(SoundTypeToReference[eventSound]);

    EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        EventInstances.Add(eventInstance);

        return eventInstance;
    }
    */
}