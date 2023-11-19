using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
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
    [SerializeField] EventReference arcadeUIHover;
    [SerializeField] EventReference arcadeUISelect;

    [field: Header("UI")]
    [SerializeField] EventReference gameUIHover;
    [SerializeField] EventReference gameUISelect;

    [field: Header("3D Game")]
    [SerializeField] EventReference player3DFootsteps;

    public enum EventSounds { Null, CastleAmbience, CastleGlitchAmbience, ForestAmbience, ForestGlitchAmbience, PlayerJoust, PlayerDamage, PlayerGrassFootsteps, PlayerStoneFootsteps, CircuitCablePlug, CircuitCableUnplug, ArcadeOn, ArcadeOff, CircuitPanelOpen, Player3DFootsteps, ArcadeUIHover, ArcadeUISelect, UIHover, UISelect}

    Dictionary<EventSounds, EventReference> SoundTypeToReference;

    readonly Dictionary<EventSounds, EventInstance> SoundTypeToInstance = new();
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
            { EventSounds.ArcadeUIHover, arcadeUIHover },
            { EventSounds.ArcadeUISelect, arcadeUISelect },
            { EventSounds.UIHover, gameUIHover },
            { EventSounds.UISelect, gameUISelect },
        };
    }

    void OnDestroy()
    {
        CleanUp();
    }

    public static void TriggerAudioClip(EventSounds sound, GameObject origin) => TriggerAudioClip(sound, origin.transform.position);

    //Change this to be a static function that uses value
    public static void TriggerAudioClip(EventSounds sound, Transform origin) => TriggerAudioClip(sound, origin.position);

    public static void TriggerAudioClip(EventSounds sound, Vector3 origin)
    {
        //Debug.Log($"Triggered Audio Clip: {key}");

        if (sound == EventSounds.Null)
            return;

        if (Instance == null)
            return;

        Instance.TriggerAudioClip(Instance.SoundTypeToReference[sound], origin);
    }

    void TriggerAudioClip(EventReference sound, Vector3 origin) => RuntimeManager.PlayOneShot(sound, origin);

    EventInstance CreateEventInstance(EventSounds key, EventReference reference)
    {
        EventInstance value = RuntimeManager.CreateInstance(reference);

        EventInstances.Add(value);
        SoundTypeToInstance.Add(key, value);

        return value;
    }

    //Starts and Stops a given event instance
    public void StartEventInstance(EventSounds key, bool start = true, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        //If no event instance exists, we create a new instance and recur
        if (!SoundTypeToInstance.TryGetValue(key, out EventInstance instance))
        {
            if (!SoundTypeToReference.TryGetValue(key, out EventReference reference))
                return;

            var newInstance = CreateEventInstance(key, reference);
            
            Debug.Log($"Created new EventInstance (key) {key} | (reference) {reference} | (instnace) {newInstance}");

            StartEventInstance(key, start, stopMode);

            return;
        }

        if (start)
            instance.start();
        else
            instance.stop(stopMode);

        Debug.Log($"{(start ? "Started" : "Stopped")} {key}");
    }

    

    void CleanUp()
    {
        foreach (var eventInstance in EventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
}